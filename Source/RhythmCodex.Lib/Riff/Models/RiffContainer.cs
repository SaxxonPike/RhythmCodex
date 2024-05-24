using System.Collections.Generic;

namespace RhythmCodex.Riff.Models;

public class RiffContainer : IRiffContainer
{
    public List<IRiffChunk> Chunks { get; set; }
    public string Format { get; set; }
}