using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Ssq.Mappers;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class SsqEventDecoder : ISsqEventDecoder
    {
        private readonly IStepEventDecoder _stepEventDecoder;
        private readonly ITimingEventDecoder _timingEventDecoder;
        private readonly ITriggerEventDecoder _triggerEventDecoder;

        public SsqEventDecoder(
            ITimingEventDecoder timingEventDecoder,
            IStepEventDecoder stepEventDecoder,
            ITriggerEventDecoder triggerEventDecoder)
        {
            _timingEventDecoder = timingEventDecoder;
            _stepEventDecoder = stepEventDecoder;
            _triggerEventDecoder = triggerEventDecoder;
        }

        public IList<IEvent> Decode(
            TimingChunk timings,
            IEnumerable<Step> steps,
            IEnumerable<Trigger> triggers,
            IPanelMapper panelMapper)
        {
            return _timingEventDecoder.Decode(timings)
                .Concat(_stepEventDecoder.Decode(steps, panelMapper))
                .Concat(_triggerEventDecoder.Decode(triggers))
                .OrderBy(ev => ev[NumericData.MetricOffset])
                .AsList();
        }
    }
}