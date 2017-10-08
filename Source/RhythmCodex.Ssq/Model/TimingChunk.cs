using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ssq.Model
{
    [Model]
    public class TimingChunk
    {
        public IEnumerable<Timing> Timings { get; set; }
        public int Rate { get; set; }
    }
}