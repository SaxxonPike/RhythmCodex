using System.Collections.Generic;
using System.IO;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Streamers
{
    public interface IDjmainChartEventStreamWriter
    {
        int Write(Stream stream, IEnumerable<IDjmainChartEvent> events);
    }
}