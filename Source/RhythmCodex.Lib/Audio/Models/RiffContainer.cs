using System.Collections.Generic;

namespace RhythmCodex.Audio.Models
{
    public class RiffContainer : IRiffContainer
    {
        public IList<IRiffChunk> Chunks { get; set; }
        public string Format { get; set; }
    }
}