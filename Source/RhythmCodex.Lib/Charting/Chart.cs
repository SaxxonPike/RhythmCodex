using System.Collections.Generic;
using Numerics;

namespace RhythmCodex.Charting
{
    public class Chart : Metadata, IChart
    {
        public IList<IEvent> Events { get; set; }
    }
}
