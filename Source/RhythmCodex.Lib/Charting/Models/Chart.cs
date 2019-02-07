using System.Collections.Generic;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Charting.Models
{
    [Model]
    public class Chart : Metadata, IChart
    {
        public IList<IEvent> Events { get; set; }
    }
}