using System.Collections.Generic;

namespace RhythmCodex.Riff.Models;

public interface IRiffContainer
{
    string Format { get; }
    List<IRiffChunk> Chunks { get; }
}