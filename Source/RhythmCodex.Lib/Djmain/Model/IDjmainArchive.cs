using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Djmain.Model
{
    public interface IDjmainArchive
    {
        int Id { get; }
        IList<Chart> Charts { get; }
        IList<ISound> Samples { get; }
        IDictionary<int, IEnumerable<IDjmainChartEvent>> RawCharts { get; set; }
    }
}