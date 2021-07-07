using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ITimingEventEncoder
    {
        TimingChunk Encode(IEnumerable<Event> events, int linearRate, BigRational metricLength, BigRational? offset, BigRational? startBpm);
    }
}