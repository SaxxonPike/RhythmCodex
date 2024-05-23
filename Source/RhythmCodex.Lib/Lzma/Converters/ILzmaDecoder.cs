using System.IO;

namespace RhythmCodex.Lzma.Converters;

public interface ILzmaDecoder
{
    byte[] Decode(Stream baseStream, int compressedLength, int decompressedLength, byte[]? decoderProperties = null);
}