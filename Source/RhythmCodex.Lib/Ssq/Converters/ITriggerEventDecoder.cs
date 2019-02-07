using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Charting.Models;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ITriggerEventDecoder
    {
        IEnumerable<IEvent> Decode(IEnumerable<Trigger> triggers);
    }
}