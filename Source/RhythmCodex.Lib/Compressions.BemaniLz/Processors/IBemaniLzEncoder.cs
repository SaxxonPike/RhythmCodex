using System;

namespace RhythmCodex.Compressions.BemaniLz.Processors;

public interface IBemaniLzEncoder
{
    Memory<byte> Encode(ReadOnlySpan<byte> source);
}