using System.Collections.Generic;
using RhythmCodex.Beatmania.Ps2.Models;
using RhythmCodex.Charting.Models;

namespace RhythmCodex.Beatmania.Ps2.Converters;

public interface IBeatmaniaPs2EventDecoder
{
    List<Event> Decode(BeatmaniaPs2Event ev);
}