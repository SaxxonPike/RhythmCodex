using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class SsqChunkFilter : ISsqChunkFilter
    {
        private readonly IStepChunkDecoder _stepChunkDecoder;
        private readonly ILogger _logger;
        private readonly ITimingChunkDecoder _timingChunkDecoder;
        private readonly ITriggerChunkDecoder _triggerChunkDecoder;

        public SsqChunkFilter(
            ITimingChunkDecoder timingChunkDecoder,
            ITriggerChunkDecoder triggerChunkDecoder,
            IStepChunkDecoder stepChunkDecoder,
            ILogger logger)
        {
            _timingChunkDecoder = timingChunkDecoder;
            _triggerChunkDecoder = triggerChunkDecoder;
            _stepChunkDecoder = stepChunkDecoder;
            _logger = logger;
        }

        public TimingChunk GetTimings(IEnumerable<Chunk> chunks)
        {
            int? rate = null;
            var result = new TimingChunk
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
            _logger.Debug($"BPM precision is {result.Rate} ticks/second");
            return result;
        }

        public IEnumerable<Trigger> GetTriggers(IEnumerable<Chunk> chunks)
        {
            var result = chunks.Where(c => c.Parameter0 == Parameter0.Triggers)
                .SelectMany(tc => _triggerChunkDecoder.Convert(tc.Data))
                .AsList();
            return result;
        }

        public IEnumerable<StepChunk> GetSteps(IEnumerable<Chunk> chunks)
        {
            var result = chunks.Where(c => c.Parameter0 == Parameter0.Steps)
                .Select(c => new StepChunk
                {
                    Steps = _stepChunkDecoder.Convert(c.Data),
                    Id = c.Parameter1
                }).AsList();
            foreach (var chunk in result)
            {
                _logger.Debug($"Found chart ID {chunk.Id:X4} with step count of {chunk.Steps.Count}");
            }
            return result;
        }
    }
}