using System.Collections.Generic;
using System.IO;
using RhythmCodex.Games.Beatmania.Psx.Models;

namespace RhythmCodex.Games.Beatmania.Psx.Streamers;

public interface IBeatmaniaPsxBmDataStreamReader
{
    List<BeatmaniaPsxFolder> Read(Stream stream, int length);
}