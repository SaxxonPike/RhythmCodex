using System.Collections.Generic;
using System.IO;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Beatmania.Streamers;

public interface IBeatmaniaPsxChartEventStreamReader
{
    List<DjmainChartEvent> Read(Stream stream, int length);
}