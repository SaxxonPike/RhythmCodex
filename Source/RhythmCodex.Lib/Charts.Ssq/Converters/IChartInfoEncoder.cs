using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Ssq.Converters
{
    public interface IChartInfoEncoder
    {
        ChartInfo Suggest(IEnumerable<Event> events);
        int Encode(ChartInfo info);
    }
}