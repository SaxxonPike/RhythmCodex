using System.Collections.Generic;
using JetBrains.Annotations;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Ssq.Mappers;
using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Ssq.Converters;

[PublicAPI]
public interface IStepEventDecoder
{
    List<Event> Decode(IEnumerable<Step> steps, IPanelMapper panelMapper);
}