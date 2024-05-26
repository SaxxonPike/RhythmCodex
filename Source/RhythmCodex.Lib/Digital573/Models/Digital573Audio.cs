using System;

namespace RhythmCodex.Digital573.Models;

public record Digital573Audio
{
    public ReadOnlyMemory<byte> Data { get; init; }
    public ReadOnlyMemory<byte> Key { get; init; }
    public int Counter { get; init; }
}