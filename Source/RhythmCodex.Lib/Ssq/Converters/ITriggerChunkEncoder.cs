using System;
using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ITriggerChunkEncoder
    {
        Memory<byte> Convert(IEnumerable<Trigger> triggers);
    }
}