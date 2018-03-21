﻿using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ITimingEventDecoder
    {
        IEnumerable<IEvent> Decode(TimingChunk timingChunk);
    }
}