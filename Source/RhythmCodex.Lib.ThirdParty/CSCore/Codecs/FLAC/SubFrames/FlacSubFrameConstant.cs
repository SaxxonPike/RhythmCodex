// ReSharper disable once CheckNamespace
namespace CSCore.Codecs.FLAC
{
    internal sealed class FlacSubFrameConstant : FlacSubFrameBase
    {
        public FlacSubFrameConstant(FlacBitReader reader, FlacFrameHeader header, FlacSubFrameData data, int bitsPerSample)
            : base(header)
        {
            var value = (int)reader.ReadBits(bitsPerSample);
            data.DestinationBuffer.Span.Fill(value);
        }
    }
}