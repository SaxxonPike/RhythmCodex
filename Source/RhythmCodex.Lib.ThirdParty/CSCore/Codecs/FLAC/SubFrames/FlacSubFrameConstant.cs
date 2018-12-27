// ReSharper disable once CheckNamespace
namespace CSCore.Codecs.FLAC
{
    internal sealed class FlacSubFrameConstant : FlacSubFrameBase
    {
        public FlacSubFrameConstant(FlacBitReader reader, FlacFrameHeader header, FlacSubFrameData data, int bitsPerSample)
            : base(header)
        {
            var value = (int)reader.ReadBits(bitsPerSample);

            unsafe
            {
                var pDestinationBuffer = data.DestinationBuffer.Span;
                for (var i = 0; i < header.BlockSize; i++)
                {
                    pDestinationBuffer[i] = value;
                }
            }
        }
    }
}