using System;
using RhythmCodex.Plugin.CSCore.Lib.Codecs.FLAC;
using RhythmCodex.Plugin.CSCore.Lib.Utils;

// ReSharper disable once CheckNamespace
namespace CSCore.Codecs.FLAC;

// ReSharper disable once InconsistentNaming
internal sealed class FlacSubFrameLPC : FlacSubFrameBase
{
    public FlacSubFrameLPC(FlacBitReader reader, FlacFrameHeader header, FlacSubFrameData data, int bitsPerSample,
        int order)
        : base(header)
    {
        unchecked
        {
            var resi = data.ResidualBuffer.Span;
            var dest = data.DestinationBuffer.Span;

            for (var i = 0; i < order; i++)
                resi[i] = reader.ReadBitsSigned(bitsPerSample);

            var coefPrecision = (int) reader.ReadBits(4);
            if (coefPrecision == 0x0F)
                throw new FlacException(
                    "Invalid \"quantized linear predictor coefficients' precision in bits\" was invalid. Must not be 0x0F.",
                    FlacLayer.SubFrame);
            coefPrecision += 1;

            var shiftNeeded = reader.ReadBitsSigned(5);
            if (shiftNeeded < 0)
                throw new FlacException(
                    "'\"Quantized linear predictor coefficient shift needed in bits\" was negative.",
                    FlacLayer.SubFrame);

            var q = new int[order];
            for (var i = 0; i < order; i++)
                q[i] = reader.ReadBitsSigned(coefPrecision);

            //decode the residual
            new FlacResidual(reader, header, data, order);
            resi[..order].CopyTo(dest);

            var blockSizeToProcess = header.BlockSize - order;

            if (bitsPerSample + coefPrecision + Log2(order) <= 32)
            {
                RestoreLPCSignal32(resi, dest, blockSizeToProcess, order, q, shiftNeeded);
            }
            else
            {
                RestoreLPCSignal64(resi, dest, blockSizeToProcess, order, q, shiftNeeded);
            }
        }
    }

    /// <summary>
    /// Copied from http://stackoverflow.com/questions/8970101/whats-the-quickest-way-to-compute-log2-of-an-integer-in-c 14.01.2015
    /// </summary>
    private int Log2(int x)
    {
        unchecked
        {
            var bits = 0;
            while (x > 0)
            {
                bits++;
                x >>= 1;
            }

            return bits;
        }
    }

    private static void RestoreLPCSignal32(ReadOnlySpan<int> residual, Span<int> destination, int length, int order,
        ReadOnlySpan<int> qlpCoeff,
        int lpcShiftNeeded)
    {
        unchecked
        {
            var ord1 = order - 1;
            var idx = order;

            for (var i = 0; i < length; i++)
            {
                var z = 0;
                var m = i;
                for (var k = ord1; k >= 0; k--)
                    z += qlpCoeff[k] * destination[m++];
                destination[idx] = residual[idx] + (z >> lpcShiftNeeded);
                idx++;
            }
        }
    }

    private static void RestoreLPCSignal64(ReadOnlySpan<int> residual, Span<int> destination, int length, int order,
        ReadOnlySpan<int> qlpCoeff,
        int lpcShiftNeeded)
    {
        unchecked
        {
            var ord1 = order - 1;
            var idx = order;

            for (var i = 0; i < length; i++)
            {
                var z = 0L;
                var m = i;
                for (var k = ord1; k >= 0; k--)
                    z += qlpCoeff[k] * destination[m++];
                destination[idx] = (int) (residual[idx] + (z >> lpcShiftNeeded));
                idx++;
            }
        }
    }
}