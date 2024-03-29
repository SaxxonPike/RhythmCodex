﻿using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class SsqChunkFilter : ISsqChunkFilter
    {
        private readonly IStepChunkDecoder _stepChunkDecoder;
        private readonly ISsqInfoChunkDecoder _ssqInfoChunkDecoder;
        private readonly ILogger _logger;
        private readonly ITimingChunkDecoder _timingChunkDecoder;
        private readonly ITriggerChunkDecoder _triggerChunkDecoder;

        public SsqChunkFilter(
            ITimingChunkDecoder timingChunkDecoder,
            ITriggerChunkDecoder triggerChunkDecoder,
            IStepChunkDecoder stepChunkDecoder,
            ISsqInfoChunkDecoder ssqInfoChunkDecoder,
            ILogger logger)
        {
            _timingChunkDecoder = timingChunkDecoder;
            _triggerChunkDecoder = triggerChunkDecoder;
            _stepChunkDecoder = stepChunkDecoder;
            _ssqInfoChunkDecoder = ssqInfoChunkDecoder;
            _logger = logger;
        }

        public TimingChunk GetTimings(IEnumerable<SsqChunk> chunks)
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

        public IList<Trigger> GetTriggers(IEnumerable<SsqChunk> chunks)
        {
            var result = chunks.Where(c => c.Parameter0 == Parameter0.Triggers)
                .SelectMany(tc => _triggerChunkDecoder.Convert(tc.Data))
                .AsList();
            return result;
        }

        public IList<StepChunk> GetSteps(IEnumerable<SsqChunk> chunks)
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

        public IList<SsqInfoChunk> GetInfos(IEnumerable<SsqChunk> chunks)
        {
            var result = chunks.Where(c => c.Parameter0 == Parameter0.Meta)
                .Select(c => _ssqInfoChunkDecoder.Decode(c))
                .AsList();
            return result;
        }
    }
}