using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ISsqChunkFilter
    {
        TimingChunk GetTimings(IEnumerable<SsqChunk> chunks);
        IList<Trigger> GetTriggers(IEnumerable<SsqChunk> chunks);
        IList<StepChunk> GetSteps(IEnumerable<SsqChunk> chunks);
        IList<SsqInfoChunk> GetInfos(IEnumerable<SsqChunk> chunks);
    }
}