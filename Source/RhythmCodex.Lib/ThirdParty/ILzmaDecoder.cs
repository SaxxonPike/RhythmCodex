using System.IO;

namespace RhythmCodex.ThirdParty
{
    public interface ILzmaDecoder
    {
        byte[] Decode(Stream baseStream, int compressedLength, int decompressedLength, byte[] decoderProperties = null);
    }
}