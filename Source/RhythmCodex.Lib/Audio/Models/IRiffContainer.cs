using System.Collections.Generic;

namespace RhythmCodex.Audio.Models
{
    public interface IRiffContainer
    {
        string Format { get; }
        IList<IRiffChunk> Chunks { get; }
    }
}