using System;

namespace RhythmCodex.Riff.Models;

public record RiffChunk
{
    public string? Id { get; init; }
    public ReadOnlyMemory<byte> Data { get; init; }
}