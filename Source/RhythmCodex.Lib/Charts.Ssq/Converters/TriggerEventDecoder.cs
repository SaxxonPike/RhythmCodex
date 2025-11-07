using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Ssq.Converters;

[Service]
public class TriggerEventDecoder : ITriggerEventDecoder
{
    public List<Event> Decode(IEnumerable<Trigger> triggers) =>
        triggers.Select(trigger => new Event
        {
            [NumericData.MetricOffset] = (BigRational) trigger.MetricOffset / SsqConstants.MeasureLength,
            [NumericData.Trigger] = trigger.Id
        }).ToList();
}