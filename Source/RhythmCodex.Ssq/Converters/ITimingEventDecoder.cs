using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ITimingEventDecoder
    {
        IEnumerable<IEvent> Decode(IEnumerable<Timing> timings, int ticksPerSecond);
    }
}