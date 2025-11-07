using System.Collections.Generic;
using RhythmCodex.Games.Stepmania.Model;

namespace RhythmCodex.Games.Stepmania.Converters;

/// <summary>
///     Decodes key=value pairs for tags such as BPMs and Stops.
/// </summary>
public interface ITimedCommandStringDecoder
{
    /// <summary>
    ///     Decode key=value pairs for the specified event string.
    /// </summary>
    List<TimedEvent> Decode(string events);
}