// ReSharper disable once CheckNamespace
namespace CSCore.Codecs.FLAC
{
    internal sealed class FlacSubFrameVerbatim : FlacSubFrameBase
    {
        public FlacSubFrameVerbatim(FlacBitReader reader, FlacFrameHeader header, FlacSubFrameData data, int bitsPerSample)
            : base(header)
        {
            unsafe
            {
                int* ptrDest = data.DestinationBuffer, ptrResidual = data.ResidualBuffer;

                for (var i = 0; i < header.BlockSize; i++)
                {
                    var x = (int)reader.ReadBits(bitsPerSample);
                    *ptrDest++ = x;
                    *ptrResidual++ = x;
                }
            }
        }
    }
}