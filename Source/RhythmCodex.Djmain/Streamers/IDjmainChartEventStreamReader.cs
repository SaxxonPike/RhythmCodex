using System.Collections.Generic;
using System.IO;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Streamers
{
    public interface IDjmainChartEventStreamReader
    {
        IEnumerable<DjmainChartEvent> Read(Stream stream);
    }
}