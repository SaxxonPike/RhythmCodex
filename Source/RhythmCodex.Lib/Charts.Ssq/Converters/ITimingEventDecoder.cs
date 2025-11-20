using System.Collections.Generic;
using JetBrains.Annotations;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Ssq.Converters;

[PublicAPI]
public interface ITimingEventDecoder
{
    List<Event> Decode(TimingChunk timingChunk);
}