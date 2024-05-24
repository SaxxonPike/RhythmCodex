using System.Collections.Generic;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Charting.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Beatmania.Converters;

[Service]
public class BeatmaniaPs2ChartDecoder : IBeatmaniaPs2ChartDecoder
{
    public Chart Decode(IEnumerable<BeatmaniaPs2Event> events)
    {
        throw new System.NotImplementedException();
    }
}