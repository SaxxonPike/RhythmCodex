using System.Collections.Generic;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    public interface ITimedCommandStringEncoder
    {
        string Encode(IEnumerable<TimedEvent> events);
    }
}