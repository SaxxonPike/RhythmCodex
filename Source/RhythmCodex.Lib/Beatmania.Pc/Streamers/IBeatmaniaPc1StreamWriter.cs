using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Pc.Models;

namespace RhythmCodex.Beatmania.Pc.Streamers;

public interface IBeatmaniaPc1StreamWriter
{
    void Write(Stream target, IEnumerable<BeatmaniaPc1Chart> charts);
}