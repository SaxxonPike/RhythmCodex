// ReSharper disable once CheckNamespace

using RhythmCodex.Plugin.CSCore.Lib.Codecs.FLAC;
using RhythmCodex.Plugin.CSCore.Lib.Utils;

namespace CSCore.Codecs.FLAC
{
    internal sealed class FlacResidual
    {
        public FlacResidual(FlacBitReader reader, FlacFrameHeader header, FlacSubFrameData data, int order)
        {
            var codingMethod = (FlacResidualCodingMethod)reader.ReadBits(2); // 2 Bit

            if (codingMethod == FlacResidualCodingMethod.PartitionedRice || codingMethod == FlacResidualCodingMethod.PartitionedRice2)
            {
                var partitionOrder = (int)reader.ReadBits(4); //"Partition order." see https://xiph.org/flac/format.html#partitioned_rice and https://xiph.org/flac/format.html#partitioned_rice2

                FlacPartitionedRice.ProcessResidual(reader, header, data, order, partitionOrder, codingMethod);
            }
            else
            {
                throw new FlacException("Not supported RICE-Coding-Method. Stream unparseable!", FlacLayer.SubFrame);
            }
        }
    }
}