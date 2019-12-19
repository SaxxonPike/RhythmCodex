// ReSharper disable once CheckNamespace

using RhythmCodex.Plugin.CSCore.Lib.Codecs.FLAC;
using RhythmCodex.Plugin.CSCore.Lib.Utils;

namespace CSCore.Codecs.FLAC
{
    internal sealed class FlacSubFrameVerbatim : FlacSubFrameBase
    {
        public FlacSubFrameVerbatim(FlacBitReader reader, FlacFrameHeader header, FlacSubFrameData data,
            int bitsPerSample)
            : base(header)
        {
            unchecked
            {
                var dest = data.DestinationBuffer.Span;
                var resi = data.ResidualBuffer.Span;

                for (var i = 0; i < header.BlockSize; i++)
                {
                    var x = (int) reader.ReadBits(bitsPerSample);
                    dest[i] = resi[i] = x;
                }
            }
        }
    }
}