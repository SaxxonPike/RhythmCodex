using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

[Service]
public class BeatmaniaPs2ChartDecoder : IBeatmaniaPs2ChartDecoder
{
    public Chart Decode(IEnumerable<BeatmaniaPs2Event> events)
    {
        throw new System.NotImplementedException();
    }
}