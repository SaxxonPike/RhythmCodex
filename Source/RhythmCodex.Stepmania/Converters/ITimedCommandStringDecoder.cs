using System.Collections.Generic;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    /// <summary>
    /// Decodes key=value pairs for tags such as BPMs and Stops.
    /// </summary>
    public interface ITimedCommandStringDecoder
    {
        /// <summary>
        /// Decode key=value pairs for the specified event string.
        /// </summary>
        IEnumerable<TimedEvent> Decode(string events);
    }
}