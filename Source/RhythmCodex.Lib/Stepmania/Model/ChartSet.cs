using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Stepmania.Model
{
    [Model]
    public class ChartSet
    {
        public IMetadata Metadata { get; set; }
        public IList<Chart> Charts { get; set; }
    }
}