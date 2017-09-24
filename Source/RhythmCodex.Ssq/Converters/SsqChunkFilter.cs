using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class SsqChunkFilter : ISsqChunkFilter
    {
        private readonly IStepChunkDecoder _stepChunkDecoder;
        private readonly ITimingChunkDecoder _timingChunkDecoder;
        private readonly ITriggerChunkDecoder _triggerChunkDecoder;

        public SsqChunkFilter(
            ITimingChunkDecoder timingChunkDecoder,
            ITriggerChunkDecoder triggerChunkDecoder,
            IStepChunkDecoder stepChunkDecoder)
        {
            _timingChunkDecoder = timingChunkDecoder;
            _triggerChunkDecoder = triggerChunkDecoder;
            _stepChunkDecoder = stepChunkDecoder;
        }

        public TimingChunk GetTimings(IEnumerable<Chunk> chunks)
        {
            int? rate = null;
            return new TimingChunk
            {
                Timings = chunks.Where(c => c.Parameter0 == Parameter0.Timings)
                    .SelectMany(tc =>
                    {
                        rate = rate ?? tc.Parameter1;
                        return _timingChunkDecoder.Convert(tc.Data);
                    })
                    .AsList(),
                Rate = rate ?? 75
            };
        }

        public IEnumerable<Trigger> GetTriggers(IEnumerable<Chunk> chunks)
        {
            return chunks.Where(c => c.Parameter0 == Parameter0.Triggers)
                .SelectMany(tc => _triggerChunkDecoder.Convert(tc.Data))
                .AsList();
        }

        public IEnumerable<StepChunk> GetSteps(IEnumerable<Chunk> chunks)
        {
            return chunks.Where(c => c.Parameter0 == Parameter0.Steps)
                .Select(c => new StepChunk
                {
                    Steps = _stepChunkDecoder.Convert(c.Data),
                    Id = c.Parameter1
                }).AsList();
        }
    }
}