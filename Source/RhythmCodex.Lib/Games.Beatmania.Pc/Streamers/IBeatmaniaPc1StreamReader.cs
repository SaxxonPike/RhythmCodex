using System.Collections.Generic;
using System.IO;
using RhythmCodex.Games.Beatmania.Pc.Models;

namespace RhythmCodex.Games.Beatmania.Pc.Streamers;

public interface IBeatmaniaPc1StreamReader
{
    IEnumerable<BeatmaniaPc1Chart> Read(Stream source, long length);
}