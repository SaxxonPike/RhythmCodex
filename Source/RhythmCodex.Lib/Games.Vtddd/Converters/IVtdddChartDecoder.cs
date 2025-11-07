using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Vtddd.Models;

namespace RhythmCodex.Games.Vtddd.Converters;

public interface IVtdddChartDecoder
{
    Chart Decode(IEnumerable<VtdddStep> steps);
}