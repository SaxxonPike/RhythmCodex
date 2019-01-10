using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Models;

namespace RhythmCodex.Beatmania.Streamers
{
    public interface IBeatmaniaPsxBmDataStreamReader
    {
        IList<BeatmaniaPsxFolder> Read(Stream stream, int length);
    }
}