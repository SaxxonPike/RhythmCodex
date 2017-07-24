using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ITriggerChunkDecoder
    {
        List<Trigger> Convert(byte[] data);
    }
}