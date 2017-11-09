using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Stepmania.Model
{
    [Model]
    public class ChartSet
    {
        public IMetadata Metadata { get; set; }
        public IList<IChart> Charts { get; set; }
    }
}