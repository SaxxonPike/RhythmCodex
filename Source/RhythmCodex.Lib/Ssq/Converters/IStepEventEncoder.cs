using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Ssq.Mappers;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface IStepEventEncoder
    {
        IList<Step> Encode(IEnumerable<Event> events, IPanelMapper panelMapper);
    }
}