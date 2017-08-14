using System.Collections.Generic;
using RhythmCodex.Attributes;

namespace RhythmCodex.Charting
{
    public class Chart : Metadata, IChart
    {
        public IList<IEvent> Events { get; set; }
    }
}
