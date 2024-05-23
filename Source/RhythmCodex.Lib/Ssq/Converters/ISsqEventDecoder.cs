using System.Collections.Generic;
using JetBrains.Annotations;
using RhythmCodex.Charting.Models;
using RhythmCodex.Ssq.Mappers;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[PublicAPI]
public interface ISsqEventDecoder
{
    List<Event> Decode(
        TimingChunk timings,
        IEnumerable<Step> steps,
        IEnumerable<Trigger> triggers,
        IPanelMapper panelMapper);
}