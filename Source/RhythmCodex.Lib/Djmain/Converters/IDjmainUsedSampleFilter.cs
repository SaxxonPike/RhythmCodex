using System.Collections.Generic;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters
{
    public interface IDjmainUsedSampleFilter
    {
        IDictionary<int, IDjmainSampleInfo> Filter(IDictionary<int, IDjmainSampleInfo> samples,
            IEnumerable<IDjmainChartEvent> events);
    }
}