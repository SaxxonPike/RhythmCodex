using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

public interface ISsqChunkFilter
{
    TimingChunk GetTimings(IEnumerable<SsqChunk> chunks);
    List<Trigger> GetTriggers(IEnumerable<SsqChunk> chunks);
    List<StepChunk> GetSteps(IEnumerable<SsqChunk> chunks);
    List<SsqInfoChunk> GetInfos(IEnumerable<SsqChunk> chunks);
}