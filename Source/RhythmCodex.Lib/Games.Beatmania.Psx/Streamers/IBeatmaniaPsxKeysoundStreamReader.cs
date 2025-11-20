using System.Collections.Generic;
using System.IO;
using RhythmCodex.Games.Beatmania.Psx.Models;

namespace RhythmCodex.Games.Beatmania.Psx.Streamers;

public interface IBeatmaniaPsxKeysoundStreamReader
{
    List<BeatmaniaPsxKeysound> Read(Stream stream);
}