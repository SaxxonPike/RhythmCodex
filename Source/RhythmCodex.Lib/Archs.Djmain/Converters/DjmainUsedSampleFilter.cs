using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Djmain.Converters;

[Service]
public class DjmainUsedSampleFilter(IDjmainChartDecoder chartDecoder)
    : IDjmainUsedSampleFilter
{
    public Dictionary<int, DjmainSampleInfo> Filter(IDictionary<int, DjmainSampleInfo> samples,
        IEnumerable<DjmainChartEvent> inEvents)
    {
        var events = inEvents.AsList();
        var skip = chartDecoder.GetFirstEventOffset(events);

        return events.Skip(skip)
            .Where(IsNote)
            .Select(ev => ev.Param1 - 1)
            .Distinct()
            .Intersect(samples.Select(s => s.Key))
            .ToDictionary(i => i, i => samples[i]);
    }

    private static bool IsNote(DjmainChartEvent ev) =>
        (DjmainEventType)(ev.Param0 & 0xF) switch
        {
            DjmainEventType.SoundSelect or DjmainEventType.Bgm => true,
            _ => false
        };
}