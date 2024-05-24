using System.Collections.Generic;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Charting.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Beatmania.Converters;

[Service]
public class BeatmaniaPs2EventDecoder : IBeatmaniaPs2EventDecoder
{
    public List<Event> Decode(BeatmaniaPs2Event ev)
    {
        throw new System.NotImplementedException();
    }
}