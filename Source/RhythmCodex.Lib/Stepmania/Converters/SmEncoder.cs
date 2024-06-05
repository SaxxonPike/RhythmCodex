using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters;

[Service]
public class SmEncoder(
    INoteEncoder noteEncoder,
    INoteCommandStringEncoder noteCommandStringEncoder,
    IGrooveRadarEncoder grooveRadarEncoder,
    ITimedCommandStringEncoder timedCommandStringEncoder)
    : ISmEncoder
{
    private static readonly IEnumerable<string> TagsToEncode = new[]
    {
        ChartTag.TitleTag,
        ChartTag.ArtistTag,
        ChartTag.SubTitleTag,
        ChartTag.GenreTag,
        ChartTag.TitleTranslitTag,
        ChartTag.ArtistTranslitTag,
        ChartTag.SubTitleTranslitTag,
        ChartTag.CreditTag,
        ChartTag.BannerTag,
        ChartTag.BackgroundTag,
        ChartTag.LyricsPathTag,
        ChartTag.CdTitleTag,
        ChartTag.MusicTag,
        ChartTag.OffsetTag,
        ChartTag.SampleStartTag,
        ChartTag.SampleLengthTag,
        ChartTag.DisplayBpmTag,
        ChartTag.BgChangesTag,
        ChartTag.FgChangesTag,
        ChartTag.PreviewTag
    };

    private List<string> GetDefault(string name, ChartSet chartSet)
    {
        switch (name)
        {
            case ChartTag.DisplayBpmTag:
            {
                var bpms = GetBpmEvents(chartSet.Charts)
                    .Select(e => (double)e.Value)
                    .Where(e => !double.IsInfinity(e))
                    .Select(e => Math.Round(e))
                    .ToList();

                var validBpms = bpms.Where(e => e is > 0 and < 1000).ToList();
                if (validBpms.Count == 0)
                    validBpms = bpms;

                var min = Math.Round((decimal)validBpms.Min());
                var max = Math.Round((decimal)validBpms.Max());
                return min == max
                    ? [$"{min}"]
                    : [$"{min}:{max}"];
            }
        }

        return [];
    }

    public List<Command> Encode(ChartSet chartSet)
    {
        var chartList = chartSet.Charts;
        var chartMetadata = chartSet.Metadata ?? new Metadata();

        var metaCommands = TagsToEncode
            .Select(s => new Command
            {
                Name = s,
                Values = chartMetadata[s] is { } md
                    ? [md]
                    : GetDefault(s, chartSet)
            });

        var timingCommands = GetTimingCommands(chartList);

        var noteCommands = chartList.Select(chart => new Command
        {
            Name = ChartTag.NotesTag,
            Values =
            [
                chart[NotesCommandTag.TypeTag] ?? string.Empty,
                chart[NotesCommandTag.DescriptionTag] ?? string.Empty,
                chart[NotesCommandTag.DifficultyTag] ?? string.Empty,
                $"{(chart[NumericData.PlayLevel] ?? BigRational.One).GetWholePart()}",
                grooveRadarEncoder.Encode(chart),
                noteCommandStringEncoder.Encode(noteEncoder.Encode(chart.Events))
            ]
        });

        return metaCommands
            .Concat(timingCommands)
            .Concat(noteCommands)
            .ToList();
    }

    private static List<TimedEvent> GetBpmEvents(IEnumerable<Chart> charts)
    {
        var bpms = charts
            .SelectMany(chart => chart.Events.Where(ev => ev[NumericData.Bpm] != null))
            .GroupBy(ev => ev[NumericData.MetricOffset])
            .Select(g => g.First())
            .ToList();

        // If initial BPM can't be determined, SM does weird calculations that don't pan out,
        // so find it ourselves:
        if (bpms.All(bpm => bpm[NumericData.MetricOffset] != 0))
        {
            // Find any negative metric offsets, closest to zero
            var initialBpm = bpms
                .Where(bpm => bpm[NumericData.MetricOffset] <= 0)
                .MaxBy(bpm => bpm[NumericData.MetricOffset]);

            // If all offsets are above zero, use the first one instead
            if (initialBpm == null)
            {
                initialBpm = bpms
                    .Where(bpm => bpm[NumericData.MetricOffset] > 0)
                    .MinBy(bpm => bpm[NumericData.MetricOffset]);
            }

            // Create a zero-offset BPM so SM knows how to set the BPM
            if (initialBpm != null)
            {
                bpms.Insert(0, new Event
                {
                    [NumericData.MetricOffset] = 0,
                    [NumericData.Bpm] = initialBpm[NumericData.Bpm]
                });
            }
        }

        return bpms
            .Where(ev => ev[NumericData.MetricOffset] >= 0)
            .Select(ev =>
                new TimedEvent { Offset = ev[NumericData.MetricOffset].Value, Value = ev[NumericData.Bpm].Value })
            .ToList();
    }

    private static List<TimedEvent> GetStopEvents(IEnumerable<Chart> charts)
    {
        return charts
            .SelectMany(chart => chart.Events.Where(ev => ev[NumericData.Stop] != null))
            .GroupBy(ev => ev[NumericData.MetricOffset])
            .Select(g => g.First())
            .Select(ev =>
                new TimedEvent
                {
                    Offset = ev[NumericData.MetricOffset]!.Value,
                    Value = ev[NumericData.Stop]!.Value
                })
            .ToList();
    }

    private IEnumerable<Command> GetTimingCommands(List<Chart> charts)
    {
        yield return new Command
        {
            Name = ChartTag.BpmsTag,
            Values = [timedCommandStringEncoder.Encode(GetBpmEvents(charts))]
        };

        yield return new Command
        {
            Name = ChartTag.StopsTag,
            Values = [timedCommandStringEncoder.Encode(GetStopEvents(charts))]
        };
    }
}