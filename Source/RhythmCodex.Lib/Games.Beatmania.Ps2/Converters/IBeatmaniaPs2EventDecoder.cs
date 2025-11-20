using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Beatmania.Ps2.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

public interface IBeatmaniaPs2EventDecoder
{
    List<Event> Decode(BeatmaniaPs2Event ev);
}