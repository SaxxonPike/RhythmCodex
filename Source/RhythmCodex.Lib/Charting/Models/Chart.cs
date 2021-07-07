using System.Collections.Generic;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Charting.Models
{
    [Model]
    public class Chart : Metadata
    {
        public IList<Event> Events { get; set; }
    }
}