using System.Collections.Generic;

namespace RhythmCodex.Sounds.Riff.Models;

public class RiffContainer
{
    public List<RiffChunk> Chunks { get; set; }
    public string? Format { get; set; }
}