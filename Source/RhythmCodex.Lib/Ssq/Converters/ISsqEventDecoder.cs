using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Ssq.Mappers;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ISsqEventDecoder
    {
        IList<Event> Decode(
            TimingChunk timings,
            IEnumerable<Step> steps,
            IEnumerable<Trigger> triggers,
            IPanelMapper panelMapper);
    }
}