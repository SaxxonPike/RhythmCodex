using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Extensions;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class SsqDecoder : ISsqDecoder
    {
        private readonly ITimingChunkDecoder _timingChunkDecoder;
        private readonly ITimingEventDecoder _timingEventDecoder;
        private readonly IStepChunkDecoder _stepChunkDecoder;
        private readonly IStepEventDecoder _stepEventDecoder;
        private readonly ITriggerChunkDecoder _triggerChunkDecoder;
        private readonly ITriggerEventDecoder _triggerEventDecoder;

        public SsqDecoder(
            ITimingChunkDecoder timingDecoder,
            ITimingEventDecoder timingEventDecoder,
            IStepChunkDecoder stepChunkDecoder,
            IStepEventDecoder stepEventDecoder,
            ITriggerChunkDecoder triggerChunkDecoder,
            ITriggerEventDecoder triggerEventDecoder)
        {
            _timingChunkDecoder = timingDecoder;
            _timingEventDecoder = timingEventDecoder;
            _stepChunkDecoder = stepChunkDecoder;
            _stepEventDecoder = stepEventDecoder;
            _triggerChunkDecoder = triggerChunkDecoder;
            _triggerEventDecoder = triggerEventDecoder;
        }
        
        public IEnumerable<IChart> Decode(IEnumerable<IChunk> data)
        {
            var chunks = data.AsList();
            var timings = chunks.Where(c => c.Parameter0 == Parameter0.Timings)
                .SelectMany(tc => _timingChunkDecoder.Convert(tc.Data))
                .AsList();
            var triggers = chunks.Where(c => c.Parameter0 == Parameter0.Triggers)
                .SelectMany(tc => _triggerChunkDecoder.Convert(tc.Data))
                .AsList();
            var stepChunks = chunks.Where(c => c.Parameter0 == Parameter0.Steps)
                .AsList();
            var timingRate = chunks.First(c => c.Parameter0 == Parameter0.Timings).Parameter1;

            return stepChunks.Select(sc => DecodeChart(
                timings, 
                _stepChunkDecoder.Convert(sc.Data).AsList(),
                triggers,
                sc.Parameter1,
                timingRate));
        }

        private IChart DecodeChart(
            IEnumerable<Timing> timings,
            IEnumerable<Step> steps, 
            IEnumerable<Trigger> triggers,
            int id, 
            int ticksPerSecond)
        {
            var events = _timingEventDecoder.Decode(timings, ticksPerSecond)
                .Concat(_stepEventDecoder.Decode(steps))
                .Concat(_triggerEventDecoder.Decode(triggers))
                .OrderBy(ev => ev[NumericData.MetricOffset])
                .AsList();

            return new Chart
            {
                Events = events,
                [NumericData.Id] = id
            };
        }
    }
}
