using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface IChartInfoEncoder
    {
        ChartInfo Suggest(IEnumerable<Event> events);
        int Encode(ChartInfo info);
    }
}