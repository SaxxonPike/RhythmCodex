using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ISsqEventDecoder
    {
        IEnumerable<IEvent> Decode(
            IEnumerable<Timing> timings,
            IEnumerable<Step> steps,
            IEnumerable<Trigger> triggers,
            IPanelMapper panelMapper,
            int ticksPerSecond);
    }
}