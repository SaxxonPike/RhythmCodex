using System.Collections.Generic;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Beatmania.Converters
{
    public interface IBeatmaniaPc1ChartDecoder
    {
        Chart Decode(IEnumerable<BeatmaniaPc1Event> events, BigRational rate);
    }
}