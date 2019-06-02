using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Models;

namespace RhythmCodex.Beatmania.Streamers
{
    public interface IBeatmaniaPc1StreamWriter
    {
        void Write(Stream target, IEnumerable<BeatmaniaPc1Chart> charts);
    }
}