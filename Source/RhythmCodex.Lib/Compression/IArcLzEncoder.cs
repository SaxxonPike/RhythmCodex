using System;

namespace RhythmCodex.Compression;

public interface IArcLzEncoder
{
    byte[] Encode(ReadOnlySpan<byte> source);
}