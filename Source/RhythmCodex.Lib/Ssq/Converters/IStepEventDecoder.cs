using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Ssq.Mappers;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface IStepEventDecoder
    {
        IList<IEvent> Decode(IEnumerable<Step> steps, IPanelMapper panelMapper);
    }
}