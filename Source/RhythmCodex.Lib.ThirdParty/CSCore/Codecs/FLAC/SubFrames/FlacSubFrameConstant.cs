// ReSharper disable once CheckNamespace
namespace CSCore.Codecs.FLAC
{
    internal sealed class FlacSubFrameConstant : FlacSubFrameBase
    {
#if FLAC_DEBUG
        public int Value { get; private set; }
#endif
        public FlacSubFrameConstant(FlacBitReader reader, FlacFrameHeader header, FlacSubFrameData data, int bitsPerSample)
            : base(header)
        {
            var value = (int)reader.ReadBits(bitsPerSample);
#if FLAC_DEBUG
            Value = value;
#endif

            unsafe
            {
                var pDestinationBuffer = data.DestinationBuffer;
                for (var i = 0; i < header.BlockSize; i++)
                {
                    *pDestinationBuffer++ = value;
                }
            }
        }
    }
}