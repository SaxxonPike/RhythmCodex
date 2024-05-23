using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Models;

namespace RhythmCodex.Beatmania.Streamers;

public interface IBeatmaniaPc1StreamReader
{
    IEnumerable<BeatmaniaPc1Chart> Read(Stream source, long length);
}