using System;

namespace RhythmCodex.FileSystems.Arc.Processors;

public interface IArcLzEncoder
{
    Memory<byte> Encode(ReadOnlySpan<byte> source);
}