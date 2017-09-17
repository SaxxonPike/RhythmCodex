using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class SsqDecoder : ISsqDecoder
    {
        private readonly ISsqEventDecoder _ssqEventDecoder;
        private readonly ITimingChunkDecoder _timingChunkDecoder;
        private readonly IStepChunkDecoder _stepChunkDecoder;
        private readonly ITriggerChunkDecoder _triggerChunkDecoder;

        public SsqDecoder(
            ISsqEventDecoder ssqEventDecoder,
            ITimingChunkDecoder timingDecoder,
            IStepChunkDecoder stepChunkDecoder,
            ITriggerChunkDecoder triggerChunkDecoder)
        {
            _ssqEventDecoder = ssqEventDecoder;
            _timingChunkDecoder = timingDecoder;
            _stepChunkDecoder = stepChunkDecoder;
            _triggerChunkDecoder = triggerChunkDecoder;
        }
        
        public IList<IChart> Decode(IEnumerable<IChunk> data)
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

            var charts = stepChunks.Select(sc => new Chart
            {
                Events = _ssqEventDecoder.Decode(
                    timings,
                    _stepChunkDecoder.Convert(sc.Data),
                    triggers,
                    timingRate).AsList(),
                [NumericData.Id] = sc.Parameter1
            });

            return charts.Cast<IChart>().AsList();
        }
    }
}
