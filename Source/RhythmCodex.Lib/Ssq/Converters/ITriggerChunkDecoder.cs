using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

public interface ITriggerChunkDecoder
{
    IList<Trigger> Convert(byte[] data);
}