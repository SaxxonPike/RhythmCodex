using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Pc.Models;

namespace RhythmCodex.Beatmania.Pc.Streamers;

public interface IBeatmaniaPc1StreamReader
{
    IEnumerable<BeatmaniaPc1Chart> Read(Stream source, long length);
}