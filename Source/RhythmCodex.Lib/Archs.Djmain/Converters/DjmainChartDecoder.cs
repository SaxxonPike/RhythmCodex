using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Charts.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Archs.Djmain.Converters;

[Service]
public class DjmainChartDecoder(IDjmainEventMetadataDecoder djmainEventMetadataDecoder) : IDjmainChartDecoder
{
    public Chart Decode(IEnumerable<DjmainChartEvent> events, DjmainChartType chartType)
    {
        return new Chart
        {
            Events = DecodeEvents(events, chartType).ToList()
        };
    }

    private IEnumerable<Event> DecodeEvents(IEnumerable<DjmainChartEvent> events, DjmainChartType chartType)
    {
        var noteCount = true;

        foreach (var ev in events)
        {
            var command = ev.Param0 & 0xF;

            if (noteCount)
                if (ev.Offset != 0 || command != 0)
                    noteCount = false;
                else
                    continue;

            var offset = new BigRational(ev.Offset, 58);
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
                    djmainEventMetadataDecoder.AddBeatmaniaMetadata(newEv, ev);
                    break;
                case DjmainChartType.Popn:
                    djmainEventMetadataDecoder.AddPopnMetadata(newEv, ev);
                    break;
            }

            yield return newEv;
        }
    }
}