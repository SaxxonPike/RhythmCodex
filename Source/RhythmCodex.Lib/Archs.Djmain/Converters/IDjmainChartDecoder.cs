using System.Collections.Generic;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Charts.Models;

namespace RhythmCodex.Archs.Djmain.Converters;

public interface IDjmainChartDecoder
{
    Chart Decode(IEnumerable<DjmainChartEvent> events, DjmainChartType chartType);
    int GetFirstEventOffset(IEnumerable<DjmainChartEvent> events);
}