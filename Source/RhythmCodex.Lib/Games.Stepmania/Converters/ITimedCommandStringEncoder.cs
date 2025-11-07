using System.Collections.Generic;
using RhythmCodex.Games.Stepmania.Model;

namespace RhythmCodex.Games.Stepmania.Converters;

public interface ITimedCommandStringEncoder
{
    string Encode(IEnumerable<TimedEvent> events);
}