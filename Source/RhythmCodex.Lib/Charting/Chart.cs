using System.Collections.Generic;
using Numerics;

namespace RhythmCodex.Charting
{
    public class Chart : Metadata, IChart
    {
        public List<IEvent> Events { get; set; }
    }
}
