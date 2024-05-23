using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Vtddd.Models;

namespace RhythmCodex.Vtddd.Converters;

public interface IVtdddChartDecoder
{
    IChart Decode(IEnumerable<VtdddStep> steps);
}