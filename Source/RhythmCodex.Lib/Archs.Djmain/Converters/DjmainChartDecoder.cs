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
    public Chart Decode(IEnumerable<DjmainChartEvent> events, DjmainDecodeOptions options)
    {
        var result = DecodeEvents(events, options).ToList();

        //
        // Account for fractional BPMs in some beatmania CS games.
        //

        if (options.ChartType == DjmainChartType.BeatmaniaCs)
        {
            for (var i = 1; i < result.Count; i++)
            {
                var lastEvent = result[i - 1];
                var currentEvent = result[i];

                if (lastEvent[NumericData.Bpm] == null ||
                    currentEvent[NumericData.Bpm] == null)
                    continue;

                // Caution: this works by coincidence
                if (lastEvent[NumericData.Bpm] != currentEvent[NumericData.Bpm])
                    lastEvent[NumericData.Bpm] += currentEvent[NumericData.Bpm] / 100;

                result.RemoveAt(i);
                i--;
            }
        }

        return new Chart
        {
            Events = result
        };
    }

    public int GetFirstEventOffset(IEnumerable<DjmainChartEvent> inEvents) =>
        inEvents.TakeWhile(e => e.Offset == 0 && (e.Param0 & 0xF) == 0).Count();

    private IEnumerable<Event> DecodeEvents(IEnumerable<DjmainChartEvent> inEvents, DjmainDecodeOptions options)
    {
        var events = inEvents.AsList();
        var eventStart = GetFirstEventOffset(events);

        var timing = options.ChartTiming switch
        {
            DjmainChartTiming.HomeNtsc => 60f,
            DjmainChartTiming.HomePal => 50f,
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

            switch (options.ChartType)
            {
                case DjmainChartType.Beatmania:
                    djmainEventMetadataDecoder.AddBeatmaniaMetadata(newEv, ev, options.SwapStereo);
                    break;
                case DjmainChartType.BeatmaniaCs:
                    djmainEventMetadataDecoder.AddBeatmaniaCsMetadata(newEv, ev);
                    break;
                case DjmainChartType.Popn:
                    djmainEventMetadataDecoder.AddPopnMetadata(newEv, ev, options.SwapStereo);
                    break;
            }

            yield return newEv;
        }
    }
}