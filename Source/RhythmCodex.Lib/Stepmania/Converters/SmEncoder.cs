using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters;

[Service]
public class SmEncoder : ISmEncoder
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

    private readonly IGrooveRadarEncoder _grooveRadarEncoder;
    private readonly INoteCommandStringEncoder _noteCommandStringEncoder;
    private readonly INoteEncoder _noteEncoder;
    private readonly ITimedCommandStringEncoder _timedCommandStringEncoder;

    public SmEncoder(
        INoteEncoder noteEncoder,
        INoteCommandStringEncoder noteCommandStringEncoder,
        IGrooveRadarEncoder grooveRadarEncoder,
        ITimedCommandStringEncoder timedCommandStringEncoder)
    {
        _noteEncoder = noteEncoder;
        _noteCommandStringEncoder = noteCommandStringEncoder;
        _grooveRadarEncoder = grooveRadarEncoder;
        _timedCommandStringEncoder = timedCommandStringEncoder;
    }

    private string[] GetDefault(string name, ChartSet chartSet)
    {
        switch (name)
        {
            case ChartTag.DisplayBpmTag:
            {
                var bpms = GetBpmEvents(chartSet.Charts)
                    .Select(e => (double) e.Value)
                    .Where(e => !double.IsInfinity(e))
                    .Select(e => Math.Round(e))
                    .AsList();

                IList<double> validBpms = bpms.Where(e => e > 0 && e < 1000).ToList();
                if (!validBpms.Any())
                    validBpms = bpms;

                var min = Math.Round((decimal) bpms.Min());
                var max = Math.Round((decimal) bpms.Max());
                return (min == max)
                    ? new[] {$"{min}"}
                    : new[] {$"{min}:{max}"};
            }
        }

        return Array.Empty<string>();
    }

    public IList<Command> Encode(ChartSet chartSet)
    {
        var chartList = chartSet.Charts.AsList();
        var chartMetadata = chartSet.Metadata ?? new Metadata();

        var metaCommands = TagsToEncode
            .Select(s => new Command
            {
                Name = s,
                Values = chartMetadata[s] != null
                    ? new[] {chartMetadata[s]}
                    : GetDefault(s, chartSet)
            });

        var timingCommands = GetTimingCommands(chartList);

        var noteCommands = chartList.Select(chart => new Command
        {
            Name = ChartTag.NotesTag,
            Values = new[]
            {
                chart[NotesCommandTag.TypeTag] ?? string.Empty,
                chart[NotesCommandTag.DescriptionTag] ?? string.Empty,
                chart[NotesCommandTag.DifficultyTag] ?? string.Empty,
                $"{(chart[NumericData.PlayLevel] ?? BigRational.One).GetWholePart()}",
                _grooveRadarEncoder.Encode(chart),
                _noteCommandStringEncoder.Encode(_noteEncoder.Encode(chart.Events))
            }
        });

        return metaCommands
            .Concat(timingCommands)
            .Concat(noteCommands)
            .ToList();
    }

    private IEnumerable<TimedEvent> GetBpmEvents(IEnumerable<IChart> charts)
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
                .OrderBy(bpm => bpm[NumericData.MetricOffset])
                .LastOrDefault();

            // If all offsets are above zero, use the first one instead
            if (initialBpm == null)
            {
                initialBpm = bpms
                    .Where(bpm => bpm[NumericData.MetricOffset] > 0)
                    .OrderBy(bpm => bpm[NumericData.MetricOffset])
                    .FirstOrDefault();
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
                new TimedEvent {Offset = ev[NumericData.MetricOffset].Value, Value = ev[NumericData.Bpm].Value});
    }

    private IEnumerable<TimedEvent> GetStopEvents(IEnumerable<IChart> charts)
    {
        return charts
            .SelectMany(chart => chart.Events.Where(ev => ev[NumericData.Stop] != null))
            .GroupBy(ev => ev[NumericData.MetricOffset])
            .Select(g => g.First())
            .Select(ev =>
                new TimedEvent {Offset = ev[NumericData.MetricOffset].Value, Value = ev[NumericData.Stop].Value});
    }

    private IEnumerable<Command> GetTimingCommands(IList<IChart> charts)
    {
        yield return new Command
        {
            Name = ChartTag.BpmsTag,
            Values = new[] {_timedCommandStringEncoder.Encode(GetBpmEvents(charts))}
        };

        yield return new Command
        {
            Name = ChartTag.StopsTag,
            Values = new[] {_timedCommandStringEncoder.Encode(GetStopEvents(charts))}
        };
    }
}