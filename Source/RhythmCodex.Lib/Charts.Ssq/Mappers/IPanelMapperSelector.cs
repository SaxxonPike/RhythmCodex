using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Ssq.Mappers;

public interface IPanelMapperSelector
{
    IPanelMapper Select(IEnumerable<Step> steps, ChartInfo chartInfo);
    IPanelMapper Select(IEnumerable<Event> events, Metadata metadata);
    IPanelMapper Select(int id);
}