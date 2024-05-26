using System.Collections.Generic;

namespace RhythmCodex.Riff.Models;

public record RiffContainer
{
    public List<RiffChunk> Chunks { get; init; }
    public string? Format { get; init; }
}