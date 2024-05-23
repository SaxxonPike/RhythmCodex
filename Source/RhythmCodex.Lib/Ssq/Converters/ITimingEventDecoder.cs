using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

public interface ITimingEventDecoder
{
    IList<IEvent> Decode(TimingChunk timingChunk);
}