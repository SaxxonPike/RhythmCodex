using System;

namespace RhythmCodex.Compression;

public interface IArcLzEncoder
{
    Memory<byte> Encode(ReadOnlySpan<byte> source);
}