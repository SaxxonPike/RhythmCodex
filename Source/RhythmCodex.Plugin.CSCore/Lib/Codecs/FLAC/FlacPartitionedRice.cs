using System;
using RhythmCodex.Plugin.CSCore.Lib.Utils;

namespace RhythmCodex.Plugin.CSCore.Lib.Codecs.FLAC;

internal static class FlacPartitionedRice
{
    public static void ProcessResidual(FlacBitReader reader, FlacFrameHeader header, FlacSubFrameData data,
        int order, int partitionOrder, FlacResidualCodingMethod codingMethod)
    {
        data.Content.UpdateSize(partitionOrder);
        var isRice2 = codingMethod == FlacResidualCodingMethod.PartitionedRice2;
        var riceParameterLength = isRice2 ? 5 : 4;
        var escapeCode = isRice2 ? 31 : 15; //11111 : 1111

        var partitionCount = 1 << partitionOrder;  //2^partitionOrder -> There will be 2^order partitions. -> "order" = partitionOrder in this case

        var residualBuffer = data.ResidualBuffer.Span[order..];

        for (var p = 0; p < partitionCount; p++)
        {
            int samplesPerPartition;
            if (partitionOrder == 0)
                samplesPerPartition = header.BlockSize - order;
            else if (p > 0)
                samplesPerPartition = header.BlockSize >> partitionOrder;
            else
                samplesPerPartition = (header.BlockSize >> partitionOrder) - order;

            var riceParameter = reader.ReadBits(riceParameterLength);
            data.Content.Parameters[p] = (int)riceParameter;

            if (riceParameter >= escapeCode)
            {
                var raw = reader.ReadBits(5); //raw is always 5 bits (see ...(+5))
                data.Content.RawBits[p] = (int)raw;
                for (var i = 0; i < samplesPerPartition; i++)
                {
                    var sample = reader.ReadBitsSigned((int)raw);
                    residualBuffer[i] = sample;
                }
            }
            else
            {
                ReadFlacRiceBlock(reader, samplesPerPartition, (int)riceParameter, residualBuffer);
            }

            residualBuffer = residualBuffer[samplesPerPartition..];
        }
    }

    /// <summary>
    /// This method is based on the CUETools.NET BitReader (see http://sourceforge.net/p/cuetoolsnet/code/ci/default/tree/CUETools.Codecs/BitReader.cs)
    /// The author "Grigory Chudov" explicitly gave the permission to use the source as part of the cscore source code which got licensed under the ms-pl.
    /// </summary>
    private static void ReadFlacRiceBlock(FlacBitReader reader, int nvals, int riceParameter, Span<int> ptrDest)
    {
        var putable = FlacBitReader.UnaryTable;
        {
            var mask = (1u << riceParameter) - 1;
            if (riceParameter == 0)
            {
                for (var i = 0; i < nvals; i++)
                {
                    ptrDest[i] = reader.ReadUnarySigned();
                }
            }
            else
            {
                for (var i = 0; i < nvals; i++)
                {
                    uint bits = putable[reader.Cache >> 24];
                    var msbs = bits;

                    while (bits == 8)
                    {
                        reader.SeekBits(8);
                        bits = putable[reader.Cache >> 24];
                        msbs += bits;
                    }

                    uint uval;
                    if (riceParameter <= 16)
                    {
                        var btsk = riceParameter + (int)bits + 1;
                        uval = (msbs << riceParameter) | ((reader.Cache >> (32 - btsk)) & mask);
                        reader.SeekBits(btsk);
                    }
                    else
                    {
                        reader.SeekBits((int)(msbs & 7) + 1);
                        uval = (msbs << riceParameter) | (reader.Cache >> (32 - riceParameter));
                        reader.SeekBits(riceParameter);
                    }
                    ptrDest[i] = (int)((uval >> 1) ^ -(int)(uval & 1));
                }
            }
        }
    }
}