using System.Collections.Generic;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Charting.Models;

namespace RhythmCodex.Beatmania.Converters;

public interface IBeatmaniaPs2ChartDecoder
{
    Chart Decode(IEnumerable<BeatmaniaPs2Event> events);
}