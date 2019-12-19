// ReSharper disable once CheckNamespace

using RhythmCodex.Plugin.CSCore.Lib.Codecs.FLAC;
using RhythmCodex.Plugin.CSCore.Lib.Utils;

namespace CSCore.Codecs.FLAC
{
    internal sealed class FlacSubFrameConstant : FlacSubFrameBase
    {
        public FlacSubFrameConstant(FlacBitReader reader, FlacFrameHeader header, FlacSubFrameData data, int bitsPerSample)
            : base(header)
        {
            unchecked
            {
                var value = (int)reader.ReadBits(bitsPerSample);
                data.DestinationBuffer.Span.Fill(value);
            }
        }
    }
}