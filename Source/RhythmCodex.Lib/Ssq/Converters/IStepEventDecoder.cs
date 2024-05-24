using System.Collections.Generic;
using JetBrains.Annotations;
using RhythmCodex.Charting.Models;
using RhythmCodex.Ssq.Mappers;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[PublicAPI]
public interface IStepEventDecoder
{
    List<Event> Decode(IEnumerable<Step> steps, IPanelMapper panelMapper);
}