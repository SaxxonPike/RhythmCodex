using System;
using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

public interface ITimingChunkDecoder
{
    IList<Timing> Convert(ReadOnlyMemory<byte> data);
}