using System.Collections.Generic;

namespace RhythmCodex.Charting
{
    public class Chart : Metadata, IChart
    {
        public IList<IEvent> Events { get; set; }
    }
}
