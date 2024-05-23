using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Djmain.Converters;

[Service]
public class DjmainChartDecoder : IDjmainChartDecoder
{
    private readonly IDjmainEventMetadataDecoder _djmainEventMetadataDecoder;

    public DjmainChartDecoder(IDjmainEventMetadataDecoder djmainEventMetadataDecoder)
    {
        _djmainEventMetadataDecoder = djmainEventMetadataDecoder;
    }

    public IChart Decode(IEnumerable<IDjmainChartEvent> events, DjmainChartType chartType)
    {
        return new Chart
        {
            Events = DecodeEvents(events, chartType).ToList()
        };
    }

    private IEnumerable<IEvent> DecodeEvents(IEnumerable<IDjmainChartEvent> events, DjmainChartType chartType)
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
                [NumericData.LinearOffset] = offset,
            };

            switch (chartType)
            {
                case DjmainChartType.Beatmania:
                    _djmainEventMetadataDecoder.AddBeatmaniaMetadata(newEv, ev);
                    break;
                case DjmainChartType.Popn:
                    _djmainEventMetadataDecoder.AddPopnMetadata(newEv, ev);
                    break;
            }

            yield return newEv;
        }
    }
}