using System;

namespace RhythmCodex.Compression;

public interface IBemaniLzEncoder
{
    Memory<byte> Encode(ReadOnlySpan<byte> source);
}