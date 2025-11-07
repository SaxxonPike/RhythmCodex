using System.Collections.Generic;
using RhythmCodex.Beatmania.Ps2.Models;
using RhythmCodex.Charting.Models;

namespace RhythmCodex.Beatmania.Ps2.Converters;

public interface IBeatmaniaPs2ChartDecoder
{
    Chart Decode(IEnumerable<BeatmaniaPs2Event> events);
}