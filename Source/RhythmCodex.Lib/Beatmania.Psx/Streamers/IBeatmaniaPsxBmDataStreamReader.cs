using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Psx.Models;

namespace RhythmCodex.Beatmania.Psx.Streamers;

public interface IBeatmaniaPsxBmDataStreamReader
{
    List<BeatmaniaPsxFolder> Read(Stream stream, int length);
}