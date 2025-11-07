using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Djmain.Model;

namespace RhythmCodex.Archs.Djmain.Streamers;

public interface IDjmainChartEventStreamReader
{
    List<DjmainChartEvent> Read(Stream stream);
}