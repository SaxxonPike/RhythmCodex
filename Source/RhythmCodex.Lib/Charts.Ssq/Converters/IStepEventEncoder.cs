using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Ssq.Mappers;
using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Ssq.Converters
{
    public interface IStepEventEncoder
    {
        IList<Step> Encode(IEnumerable<Event> events, IPanelMapper panelMapper);
    }
}