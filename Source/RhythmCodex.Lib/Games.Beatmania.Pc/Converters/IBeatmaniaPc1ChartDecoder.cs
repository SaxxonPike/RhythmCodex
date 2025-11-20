using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Beatmania.Pc.Models;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Games.Beatmania.Pc.Converters;

public interface IBeatmaniaPc1ChartDecoder
{
    Chart Decode(IEnumerable<BeatmaniaPc1Event> events, BigRational rate);
}