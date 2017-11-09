using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ISsqChunkFilter
    {
        TimingChunk GetTimings(IEnumerable<Chunk> chunks);
        IEnumerable<Trigger> GetTriggers(IEnumerable<Chunk> chunks);
        IEnumerable<StepChunk> GetSteps(IEnumerable<Chunk> chunks);
    }
}