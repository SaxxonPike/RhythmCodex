using System.Collections.Generic;
using System.IO;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.BeatmaniaPsx.Streamers
{
    public interface IBeatmaniaPsxChartEventStreamReader
    {
        IList<DjmainChartEvent> Read(Stream stream, int length);
    }
}