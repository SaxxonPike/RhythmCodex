using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ISsqChunkFilter
    {
        TimingChunk GetTimings(IEnumerable<SsqChunk> chunks);
        IEnumerable<Trigger> GetTriggers(IEnumerable<SsqChunk> chunks);
        IEnumerable<StepChunk> GetSteps(IEnumerable<SsqChunk> chunks);
        IEnumerable<SsqInfoChunk> GetInfos(IEnumerable<SsqChunk> chunks);
    }
}