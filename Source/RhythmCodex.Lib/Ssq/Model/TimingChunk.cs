using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ssq.Model
{
    [Model]
    public class TimingChunk
    {
        public IList<Timing> Timings { get; init; }
        public int Rate { get; init; }
    }
}