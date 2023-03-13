using System;

namespace RhythmCodex.Compression;

public interface IBemaniLzEncoder
{
    byte[] Encode(ReadOnlySpan<byte> source);
}