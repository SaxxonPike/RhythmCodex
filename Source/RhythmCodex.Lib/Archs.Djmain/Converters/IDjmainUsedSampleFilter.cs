using System.Collections.Generic;
using RhythmCodex.Archs.Djmain.Model;

namespace RhythmCodex.Archs.Djmain.Converters;

public interface IDjmainUsedSampleFilter
{
    Dictionary<int, DjmainSampleInfo> Filter(IDictionary<int, DjmainSampleInfo> samples,
        IEnumerable<DjmainChartEvent> events);
}