using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Ssq.Mappers;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface IStepEventDecoder
    {
        IEnumerable<IEvent> Decode(IEnumerable<Step> steps, IPanelMapper panelMapper);
    }
}