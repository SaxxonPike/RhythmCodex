using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Charts.Ssq.Converters
{
    public interface ITimingEventEncoder
    {
        TimingChunk Encode(IEnumerable<Event> events, int linearRate, BigRational metricLength, BigRational? offset, BigRational? startBpm);
    }
}