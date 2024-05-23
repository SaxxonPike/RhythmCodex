using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[Service]
public class SsqChunkFilter(
    ITimingChunkDecoder timingChunkDecoder,
    ITriggerChunkDecoder triggerChunkDecoder,
    IStepChunkDecoder stepChunkDecoder,
    ISsqInfoChunkDecoder ssqInfoChunkDecoder,
    ILogger logger)
    : ISsqChunkFilter
{
    public TimingChunk GetTimings(IEnumerable<SsqChunk> chunks)
    {
        int? rate = null;
        var result = new TimingChunk
        {
            Timings = chunks.Where(c => c.Parameter0 == Parameter0.Timings)
                .SelectMany(tc =>
                {
                    rate = rate ?? tc.Parameter1;
                    return timingChunkDecoder.Convert(tc.Data);
                })
                .ToList(),
            Rate = rate ?? 75
        };
        logger.Debug($"BPM precision is {result.Rate} ticks/second");
        return result;
    }

    public List<Trigger> GetTriggers(IEnumerable<SsqChunk> chunks)
    {
        var result = chunks
            .Where(c => c.Parameter0 == Parameter0.Triggers)
            .SelectMany(tc => triggerChunkDecoder.Convert(tc.Data))
            .ToList();

        return result;
    }

    public List<StepChunk> GetSteps(IEnumerable<SsqChunk> chunks)
    {
        var result = chunks.Where(c => c.Parameter0 == Parameter0.Steps)
            .Select(c => new StepChunk
            {
                Steps = stepChunkDecoder.Convert(c.Data),
                Id = c.Parameter1
            })
            .ToList();

        foreach (var chunk in result)
        {
            logger.Debug($"Found chart ID {chunk.Id:X4} with step count of {chunk.Steps.Count}");
        }

        return result;
    }

    public List<SsqInfoChunk> GetInfos(IEnumerable<SsqChunk> chunks)
    {
        var result = chunks.Where(c => c.Parameter0 == Parameter0.Meta)
            .Select(c => ssqInfoChunkDecoder.Decode(c))
            .ToList();

        return result;
    }
}