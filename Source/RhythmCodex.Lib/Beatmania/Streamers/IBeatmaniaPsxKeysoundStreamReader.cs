using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Models;

namespace RhythmCodex.Beatmania.Streamers;

public interface IBeatmaniaPsxKeysoundStreamReader
{
    List<BeatmaniaPsxKeysound> Read(Stream stream);
}