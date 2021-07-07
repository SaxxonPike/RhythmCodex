using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters
{
    public interface IDjmainChartDecoder
    {
        Chart Decode(IEnumerable<IDjmainChartEvent> events, DjmainChartType chartType);
    }
}