using System.Collections.Generic;
using RhythmCodex.Beatmania.Ps2.Models;
using RhythmCodex.Charting.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Beatmania.Ps2.Converters;

[Service]
public class BeatmaniaPs2ChartDecoder : IBeatmaniaPs2ChartDecoder
{
    public Chart Decode(IEnumerable<BeatmaniaPs2Event> events)
    {
        throw new System.NotImplementedException();
    }
}