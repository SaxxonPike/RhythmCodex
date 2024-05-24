using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.IoC;
using RhythmCodex.Vtddd.Models;

namespace RhythmCodex.Vtddd.Converters;

[Service]
public class VtdddChartDecoder(IVtdddStepDecoder stepDecoder) : IVtdddChartDecoder
{
    public Chart Decode(IEnumerable<VtdddStep> steps)
    {
        var events = new List<Event>();
            
        var result = new Chart()
        {
            Events = events
        };

        events.AddRange(steps.SelectMany(stepDecoder.Decode));
        return result;
    }
}