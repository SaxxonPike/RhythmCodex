using System.Collections.Generic;
using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Ssq.Converters;

public interface ISsqChunkFilter
{
    TimingChunk GetTimings(IEnumerable<SsqChunk> chunks);
    List<Trigger> GetTriggers(IEnumerable<SsqChunk> chunks);
    List<StepChunk> GetSteps(IEnumerable<SsqChunk> chunks);
    List<SsqInfoChunk> GetInfos(IEnumerable<SsqChunk> chunks);
}