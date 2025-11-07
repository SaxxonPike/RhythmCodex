using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

[Service]
public class BeatmaniaPs2EventDecoder : IBeatmaniaPs2EventDecoder
{
    public List<Event> Decode(BeatmaniaPs2Event ev)
    {
        throw new System.NotImplementedException();
    }
}