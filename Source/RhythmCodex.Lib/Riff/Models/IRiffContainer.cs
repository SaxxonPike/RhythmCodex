using System.Collections.Generic;

namespace RhythmCodex.Riff.Models;

public interface IRiffContainer
{
    string Format { get; }
    IList<IRiffChunk> Chunks { get; }
}