using System.Collections.Generic;
using System.Linq;
using Numerics;
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

        public SsqDecoder(
            ITimingChunkDecoder timingDecoder,
            ITimingEventDecoder timingEventDecoder,
            IStepChunkDecoder stepChunkDecoder,
            IStepEventDecoder stepEventDecoder)
        {
            _timingChunkDecoder = timingDecoder;
            _timingEventDecoder = timingEventDecoder;
            _stepChunkDecoder = stepChunkDecoder;
            _stepEventDecoder = stepEventDecoder;
        }
        
        public IEnumerable<IChart> Decode(IEnumerable<Chunk?> data)
        {
            var chunks = data.Where(c => c.HasValue).Select(c => c.Value).AsList();
            var timingChunks = chunks.Where(c => c.Parameter0 == Parameter0.Timings);
            var stepChunks = chunks.Where(c => c.Parameter0 == Parameter0.Steps);
            var timingChunk = timingChunks.First();
            var timings = _timingChunkDecoder.Convert(timingChunk.Data).AsList();

            return stepChunks.Select(sc => DecodeChart(
                timings, 
                _stepChunkDecoder.Convert(sc.Data).AsList(), 
                sc.Parameter1,
                timingChunk.Parameter1));
        }

        private IChart DecodeChart(IEnumerable<Timing> timings, IEnumerable<Step> steps, int id, int ticksPerSecond)
        {
            var events = _timingEventDecoder.Decode(timings, ticksPerSecond)
                .Concat(_stepEventDecoder.Decode(steps))
                .ToList();

            return new Chart
            {
                Events = events,
                [NumericData.Id] = id,
            };
        }
    }
}
