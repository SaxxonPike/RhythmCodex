using System.Collections.Generic;
using System.IO;
using RhythmCodex.Games.Beatmania.Pc.Models;

namespace RhythmCodex.Games.Beatmania.Pc.Streamers;

public interface IBeatmaniaPc1StreamWriter
{
    void Write(Stream target, IEnumerable<BeatmaniaPc1Chart> charts);
}