using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Meta.Models;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Mappers
{
    public interface IPanelMapperSelector
    {
        IPanelMapper Select(IEnumerable<Step> steps, ChartInfo chartInfo);
        IPanelMapper Select(IEnumerable<Event> events, Metadata metadata);
        IPanelMapper Select(int id);
    }
}