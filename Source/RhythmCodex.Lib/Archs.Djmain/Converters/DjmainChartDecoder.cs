using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Charts.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Archs.Djmain.Converters;

[Service]
public class DjmainChartDecoder(IDjmainEventMetadataDecoder djmainEventMetadataDecoder) : IDjmainChartDecoder
{
    public Chart Decode(IEnumerable<DjmainChartEvent> events, DjmainChartType chartType, bool swapStereo)
    {
        return new Chart
        {
            Events = DecodeEvents(events, chartType, swapStereo).ToList()
        };
    }

    public int GetFirstEventOffset(IEnumerable<DjmainChartEvent> inEvents) =>
        inEvents.TakeWhile(e => e.Offset == 0 && (e.Param0 & 0xF) == 0).Count();

    private IEnumerable<Event> DecodeEvents(IEnumerable<DjmainChartEvent> inEvents, DjmainChartType chartType,
        bool swapStereo)
    {
        var events = inEvents.AsList();
        var eventStart = GetFirstEventOffset(events);

        var timing = chartType switch
        {
            DjmainChartType.BeatmaniaCs => 60f,
            _ => 58f
        };

        foreach (var ev in events.Skip(eventStart))
        {
            var offset = (BigRational)ev.Offset / timing;
            var newEv = new Event
            {
                [NumericData.SourceCommand] = ev.Param0,
                [NumericData.SourceData] = ev.Param1,
                [NumericData.SourceOffset] = ev.Offset,
                [NumericData.LinearOffset] = offset
            };

            switch (chartType)
            {
                case DjmainChartType.Beatmania:
                    djmainEventMetadataDecoder.AddBeatmaniaMetadata(newEv, ev, swapStereo);
                    break;
                case DjmainChartType.BeatmaniaCs:
                    djmainEventMetadataDecoder.AddBeatmaniaCsMetadata(newEv, ev);
                    break;
                case DjmainChartType.Popn:
                    djmainEventMetadataDecoder.AddPopnMetadata(newEv, ev, swapStereo);
                    break;
            }

            yield return newEv;
        }
    }
}