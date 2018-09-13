using System.Collections.Generic;
using System.IO;
using RhythmCodex.BeatmaniaPsx.Models;

namespace RhythmCodex.BeatmaniaPsx.Streamers
{
    public interface IBeatmaniaPsxKeysoundStreamReader
    {
        IList<BeatmaniaPsxKeysound> Read(Stream stream);
    }
}