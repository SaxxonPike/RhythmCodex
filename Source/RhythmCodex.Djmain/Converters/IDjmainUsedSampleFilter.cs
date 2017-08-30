using System.Collections.Generic;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters
{
    public interface IDjmainUsedSampleFilter
    {
        IDictionary<int, DjmainSampleInfo> Filter(IDictionary<int, DjmainSampleInfo> samples, IEnumerable<DjmainChartEvent> events);
    }
}