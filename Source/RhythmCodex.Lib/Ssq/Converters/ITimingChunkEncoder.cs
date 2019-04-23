using System;
using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ITimingChunkEncoder
    {
        byte[] Convert(IEnumerable<Timing> timings);
    }
}