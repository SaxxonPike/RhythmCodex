using System.Collections.Generic;
using RhythmCodex.Charts.Sm.Model;

namespace RhythmCodex.Charts.Sm.Converters;

public interface ITimedCommandStringEncoder
{
    string Encode(IEnumerable<TimedEvent> events);
}