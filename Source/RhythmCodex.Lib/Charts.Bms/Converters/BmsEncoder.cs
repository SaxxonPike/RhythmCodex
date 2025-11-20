using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RhythmCodex.Charts.Bms.Model;
using RhythmCodex.Charts.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Bms.Converters;

[Service]
public class BmsEncoder(ILogger logger, IBmsNoteCommandEncoder bmsNoteCommandEncoder)
    : IBmsEncoder
{
    private static CultureInfo BmsCulture => CultureInfo.InvariantCulture;

    private static readonly IReadOnlyDictionary<StringData, string> StringTagMap =
        new Dictionary<StringData, string>
        {
            {StringData.Title, "TITLE"},
            {StringData.Subtitle, "SUBTITLE"},
            {StringData.Genre, "GENRE"}
        };

    private static readonly IReadOnlyDictionary<NumericData, string> NumericTagMap =
        new Dictionary<NumericData, string>
        {
            {NumericData.Difficulty, "DIFFICULTY"},
            {NumericData.PlayLevel, "PLAYLEVEL"}
        };

    public List<BmsCommand> Encode(Chart chart, BmsEncoderOptions? options = null)
    {
        return EncodeInternal(chart, options ?? new BmsEncoderOptions()).ToList();
    }

    private IEnumerable<BmsCommand> EncodeInternal(Chart inputChart, BmsEncoderOptions options)
    {
        if (inputChart.Events.Any(ev => ev[NumericData.MetricOffset] == null))
            throw new RhythmCodexException("Metric offsets must all be populated in order to export to BMS.");

        yield return new BmsCommand {Comment = $";RhythmCodex {DateTime.Now:s}"};

        if (!inputChart.Events.Any())
            yield break;

        var chartEvents = inputChart.Events.ToList();

        // Metadata

        foreach (var kv in StringTagMap)
        {
            if (inputChart[kv.Key] != null)
                yield return new BmsCommand
                {
                    Name = kv.Value,
                    Value = inputChart[kv.Key]
                };
        }

        foreach (var kv in NumericTagMap)
        {
            if (inputChart[kv.Key] != null)
                yield return new BmsCommand
                {
                    Name = kv.Value,
                    Value = string.Format(BmsCulture, "{0}", (decimal) inputChart[kv.Key]!)
                };
        }

        // Player count (PLAYER)

        if (inputChart["PLAYER"] is null)
        {
            var playerCount = chartEvents
                .Where(ev => ev[NumericData.Player] != null &&
                             (ev[NumericData.LoadSound] != null || ev[NumericData.PlaySound] != null))
                .Select(ev => (int)ev[NumericData.Player]!.Value)
                .Distinct()
                .Count();

            yield return new BmsCommand
            {
                Name = "PLAYER",
                Value = playerCount switch
                {
                    < 2 => "1",
                    _ => "3"
                }
            };
        }
        
        // Sample definitions (WAVxx)

        var usedSamples = chartEvents
            .Where(ev => ev[NumericData.LoadSound] != null)
            .Select(ev => ev[NumericData.LoadSound])
            .Concat(chartEvents
                .Where(ev => ev[NumericData.PlaySound] != null)
                .Select(ev => ev[NumericData.PlaySound]))
            .Select(br => (int) br!)
            .Distinct()
            .ToList();

        if (usedSamples.Count > 1295)
            logger.Warning(
                $"{nameof(BmsEncoder)}: there are more than 1295 samples - WAV list will be truncated. " +
                $"{usedSamples.Count - 1295} samples will not be mapped.");

        var sampleMap = usedSamples
            .Select((e, i) => new KeyValuePair<int, int>(e, i + 1))
            .Where(kv => kv.Value is >= 0 and <= 1295)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        foreach (var kv in sampleMap)
        {
            if (kv.Key < 0)
                continue;

            string? wavName;

            if (options?.WavNameTransformer is { } transformer)
                wavName = transformer.Invoke(kv.Key);
            else
                wavName = $"{Alphabet.EncodeAlphanumeric(kv.Key, 4)}.WAV";

            if (wavName != null)
            {
                yield return new BmsCommand
                {
                    Name = $"WAV{Alphabet.EncodeAlphanumeric(kv.Value, 2)}",
                    Value = wavName
                };
            }
        }

        // BPM definitions (for BPM and xxx08:)

        var bpms = chartEvents
            .Where(ev => ev[NumericData.Bpm] != null)
            .Select(ev => ev[NumericData.Bpm]!.Value)
            .ToList();

        if (bpms.Count > 0)
        {
            yield return new BmsCommand
            {
                Name = "BPM",
                Value = $"{(decimal) bpms[0]}"
            };
        }

        var bpmMap = bpms
            .Distinct()
            .Select((e, i) => new KeyValuePair<BigRational, int>(e, i + 1))
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        foreach (var kv in bpmMap)
            yield return new BmsCommand
            {
                Name = $"BPM{Alphabet.EncodeNumeric(kv.Value, 2)}",
                Value = $"{(decimal) kv.Key}"
            };

        // Measure lengths (xxx02:)

        var measureLengths = chartEvents
            .Where(ev => (ev[FlagData.Measure] == true || ev[FlagData.End] == true)
                         && ev[NumericData.MeasureLength].HasValue)
            .GroupBy(ev => (int) ev[NumericData.MetricOffset]!.Value.GetWholePart())
            .ToDictionary(
                g => g.Key,
                g => g.First()[NumericData.MeasureLength] ?? 1);

        foreach (var kv in measureLengths.Where(ml => ml.Value != 1))
        {
            yield return new BmsCommand
            {
                Name = $"{Alphabet.EncodeNumeric(kv.Key, 3)}02",
                Value = string.Format(BmsCulture, "{0}", (decimal) kv.Value),
                UseColon = true
            };
        }

        // BPM changes

        var bpmEvents = bmsNoteCommandEncoder
            .TranslateBpmEvents(chartEvents);

        foreach (var ev in GetCommands(bpmEvents, 1920,
                     i => Alphabet.EncodeNumeric(
                         i == null
                             ? 0
                             : bpmMap.GetValueOrDefault(i.Value, 0), 2)))
            yield return ev;

        // Notes

        var noteEvents = bmsNoteCommandEncoder
            .TranslateNoteEvents(chartEvents, options!.ChartType)
            ;

        foreach (var ev in GetCommands(noteEvents, 960, i => Alphabet.EncodeAlphanumeric(
                     i == null
                         ? 0
                         : sampleMap.ContainsKey((int) i)
                             ? sampleMap[(int) i]
                             : 1295, 2)))
            yield return ev;

        // Builder

        IEnumerable<BmsCommand> GetCommands(IEnumerable<BmsEvent> events, int quantize,
            Func<BigRational?, string> encode)
        {
            var eventMeasures = events
                .GroupBy(ev => ev.Measure);

            foreach (var eventMeasure in eventMeasures)
            {
                var measure = eventMeasure.Key;
                var laneGroups = eventMeasure.GroupBy(ev => ev.Lane);
                foreach (var laneGroup in laneGroups)
                {
                    var remainingEvents = laneGroup.ToList();
                    while (remainingEvents.Any())
                    {
                        var exportableEvents = remainingEvents
                            .GroupBy(ev => ev.Offset)
                            .Select(g => g.First())
                            .ToList();

                        var exportedEvents = bmsNoteCommandEncoder.Encode(
                            exportableEvents,
                            encode,
                            measureLengths.TryGetValue(measure, out var length) ? length : 1,
                            quantize);

                        foreach (var exportableEvent in exportableEvents)
                            remainingEvents.Remove(exportableEvent);

                        yield return new BmsCommand
                        {
                            Name = string.Format(BmsCulture, "{0:D3}{1}", measure, laneGroup.Key),
                            UseColon = true,
                            Value = exportedEvents
                        };
                    }
                }
            }
        }
    }
}