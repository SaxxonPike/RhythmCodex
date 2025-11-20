using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Djmain.Model;

namespace RhythmCodex.Games.Beatmania.Psx.Streamers;

public interface IBeatmaniaPsxChartEventStreamReader
{
    List<DjmainChartEvent> Read(Stream stream, int length);
}