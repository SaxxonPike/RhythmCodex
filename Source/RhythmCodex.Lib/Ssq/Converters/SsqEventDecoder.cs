using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Ssq.Mappers;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[Service]
public class SsqEventDecoder(
    ITimingEventDecoder timingEventDecoder,
    IStepEventDecoder stepEventDecoder,
    ITriggerEventDecoder triggerEventDecoder)
    : ISsqEventDecoder
{
    public List<Event> Decode(
        TimingChunk timings,
        IEnumerable<Step> steps,
        IEnumerable<Trigger> triggers,
        IPanelMapper panelMapper)
    {
        return timingEventDecoder.Decode(timings)
            .Concat(stepEventDecoder.Decode(steps, panelMapper))
            .Concat(triggerEventDecoder.Decode(triggers))
            .OrderBy(ev => ev[NumericData.MetricOffset])
            .ToList();
    }
}