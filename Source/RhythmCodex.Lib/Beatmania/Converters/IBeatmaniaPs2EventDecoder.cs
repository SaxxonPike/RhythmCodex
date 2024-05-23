using System.Collections.Generic;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Charting.Models;

namespace RhythmCodex.Beatmania.Converters;

public interface IBeatmaniaPs2EventDecoder
{
    IList<IEvent> Decode(BeatmaniaPs2Event ev);
}