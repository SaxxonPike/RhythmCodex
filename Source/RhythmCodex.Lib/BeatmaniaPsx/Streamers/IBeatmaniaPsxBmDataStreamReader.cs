using System.Collections.Generic;
using System.IO;
using RhythmCodex.BeatmaniaPsx.Models;

namespace RhythmCodex.BeatmaniaPsx.Streamers
{
    public interface IBeatmaniaPsxBmDataStreamReader
    {
        IList<BeatmaniaPsxFolder> Read(Stream stream, int length);
    }
}