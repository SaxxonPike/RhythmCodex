using System.Collections.Generic;
using RhythmCodex.Attributes;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Charting
{
    [Model]
    public class Chart : Metadata, IChart
    {
        public IList<IEvent> Events { get; set; }
    }
}