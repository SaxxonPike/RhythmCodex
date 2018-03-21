﻿using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class TriggerEventDecoder : ITriggerEventDecoder
    {
        public IEnumerable<IEvent> Decode(IEnumerable<Trigger> triggers)
        {
            return triggers.Select(trigger => new Event
            {
                [NumericData.MetricOffset] = (BigRational) trigger.MetricOffset / SsqConstants.MeasureLength,
                [NumericData.Trigger] = trigger.Id
            });
        }
    }
}