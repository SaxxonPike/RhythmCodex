using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ITimingChunkDecoder
    {
        List<Timing> Convert(byte[] data);
    }
}