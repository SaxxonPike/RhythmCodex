using System.Collections.Generic;
using RhythmCodex.Beatmania.Pc.Models;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Beatmania.Pc.Converters;

public interface IBeatmaniaPc1ChartDecoder
{
    Chart Decode(IEnumerable<BeatmaniaPc1Event> events, BigRational rate);
}