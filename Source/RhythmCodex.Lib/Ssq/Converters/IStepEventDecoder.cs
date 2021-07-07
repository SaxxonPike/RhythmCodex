using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Ssq.Mappers;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface IStepEventDecoder
    {
        IList<Event> Decode(IEnumerable<Step> steps, IPanelMapper panelMapper);
    }
}