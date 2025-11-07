using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Psx.Models;

namespace RhythmCodex.Beatmania.Psx.Streamers;

public interface IBeatmaniaPsxKeysoundStreamReader
{
    List<BeatmaniaPsxKeysound> Read(Stream stream);
}