using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.IoC;
using RhythmCodex.Vtddd.Models;

namespace RhythmCodex.Vtddd.Converters
{
    [Service]
    public class VtdddChartDecoder : IVtdddChartDecoder
    {
        private readonly IVtdddStepDecoder _stepDecoder;

        public VtdddChartDecoder(IVtdddStepDecoder stepDecoder)
        {
            _stepDecoder = stepDecoder;
        }
        
        public IChart Decode(IEnumerable<VtdddStep> steps)
        {
            var events = new List<IEvent>();
            
            var result = new Chart()
            {
                Events = events
            };

            events.AddRange(steps.SelectMany(_stepDecoder.Decode));
            return result;
        }
    }
}