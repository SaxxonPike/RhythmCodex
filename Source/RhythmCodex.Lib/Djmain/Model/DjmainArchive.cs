using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Djmain.Model
{
    public class DjmainArchive : IDjmainArchive
    {
        public int Id { get; set; }
        public IList<IChart> Charts { get; set; }
        public IList<ISound> Samples { get; set; }
        public IDictionary<int, IEnumerable<IDjmainChartEvent>> RawCharts { get; set; }

        public override string ToString() => Json.Serialize(this);
    }
}