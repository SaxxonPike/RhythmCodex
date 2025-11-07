using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Ssq.Converters
{
    public interface ITriggerEventEncoder
    {
        IList<Trigger> Encode(IEnumerable<Event> events);
    }
}