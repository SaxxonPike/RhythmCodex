using System.IO;
using RhythmCodex.IoC;

namespace RhythmCodex.ThirdParty
{
    [Service]
    public class LzmaDecoder : ILzmaDecoder
    {
        public byte[] Decode(Stream baseStream, int compressedLength, int decompressedLength, byte[] decoderProperties = null)
        {
            var lzma = new SevenZip.Compression.LZMA.LzmaDecoder();
            
            if (decoderProperties != null)
                lzma.SetDecoderProperties(decoderProperties);
            
            using (var outStream = new MemoryStream())
            {
                lzma.Code(baseStream, outStream, compressedLength, decompressedLength, null);
                outStream.Flush();
                return outStream.ToArray();
            }
        }
    }
}