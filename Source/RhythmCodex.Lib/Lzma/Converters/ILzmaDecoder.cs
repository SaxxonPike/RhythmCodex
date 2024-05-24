using System;
using System.IO;

namespace RhythmCodex.Lzma.Converters;

public interface ILzmaDecoder
{
    Memory<byte> Decode(Stream baseStream, int compressedLength, int decompressedLength, ReadOnlySpan<byte> decoderProperties = default);
}