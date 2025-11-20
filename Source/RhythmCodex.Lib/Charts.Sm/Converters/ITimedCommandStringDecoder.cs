using System.Collections.Generic;
using RhythmCodex.Charts.Sm.Model;

namespace RhythmCodex.Charts.Sm.Converters;

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