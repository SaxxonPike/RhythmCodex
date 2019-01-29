using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Bms.Model;
using RhythmCodex.Charting;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Bms.Converters
{
    [Service]
    public class BmsEncoder : IBmsEncoder
    {
        private readonly ILogger _logger;
        private readonly IBmsNoteCommandEncoder _bmsNoteCommandEncoder;

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

        public BmsEncoder(ILogger logger, IBmsNoteCommandEncoder bmsNoteCommandEncoder)
        {
            _logger = logger;
            _bmsNoteCommandEncoder = bmsNoteCommandEncoder;
        }

        public IList<BmsCommand> Encode(IChart chart)
        {
            return EncodeInternal(chart).ToList();
        }

        private IEnumerable<BmsCommand> EncodeInternal(IChart inputChart)
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
                        Value = $"{(decimal) inputChart[kv.Key]}"
                    };
            }

            // Sample definitions (WAVxx)

            var usedSamples = chartEvents
                .Where(ev => ev[NumericData.LoadSound] != null)
                .Select(ev => ev[NumericData.LoadSound])
                .Concat(chartEvents
                    .Where(ev => ev[NumericData.PlaySound] != null)
                    .Select(ev => ev[NumericData.PlaySound]))
                .Select(br => (int) br)
                .Distinct()
                .ToList();

            if (usedSamples.Count > 1295)
                _logger.Warning(
                    $"{nameof(BmsEncoder)}: there are more than 1295 samples - WAV list will be truncated. " +
                    $"{usedSamples.Count - 1295} samples will not be mapped.");

            var sampleMap = usedSamples
                .Select((e, i) => new KeyValuePair<int, int>(e, i + 1))
                .Where(kv => kv.Value >= 0 && kv.Value <= 1295)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach (var kv in sampleMap)
            {
                yield return new BmsCommand
                {
                    Name = $"WAV{Alphabet.EncodeAlphanumeric(kv.Value, 2)}",
                    Value = $"{Alphabet.EncodeAlphanumeric(kv.Key, 4)}.WAV"
                };
            }

            // BPM definitions (for BPM and xxx08:)

            var bpms = chartEvents
                .Where(ev => ev[NumericData.Bpm] != null)
                .Select(ev => ev[NumericData.Bpm])
                .ToList();

            yield return new BmsCommand
            {
                Name = "BPM",
                Value = $"{(decimal) bpms[0]}"
            };

            var bpmMap = bpms
                .Distinct()
                .Select((e, i) => new KeyValuePair<BigRational, int>(e.Value, i + 1))
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
                .GroupBy(ev => (int) ev[NumericData.MetricOffset].Value.GetWholePart())
                .ToDictionary(
                    g => g.Key,
                    g => g.First()[NumericData.MeasureLength] ?? 1);

            foreach (var kv in measureLengths.Where(ml => ml.Value != 1))
            {
                yield return new BmsCommand
                {
                    Name = $"{Alphabet.EncodeNumeric(kv.Key, 3)}02",
                    Value = $"{(decimal) kv.Value}",
                    UseColon = true
                };
            }

            // BPM changes

            var bpmEvents = _bmsNoteCommandEncoder
                .TranslateBpmEvents(chartEvents);

            foreach (var ev in GetCommands(bpmEvents, 1920,
                i => Alphabet.EncodeNumeric(
                    i == null
                        ? 0
                        : bpmMap.ContainsKey(i.Value)
                            ? bpmMap[i.Value]
                            : 0, 2)))
                yield return ev;

            // Notes

            var noteEvents = _bmsNoteCommandEncoder
                .TranslateNoteEvents(chartEvents)
                .AsList();

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

                            var exportedEvents = _bmsNoteCommandEncoder.Encode(
                                exportableEvents,
                                encode,
                                measureLengths.ContainsKey(measure) ? measureLengths[measure] : 1,
                                quantize);

                            foreach (var exportableEvent in exportableEvents)
                                remainingEvents.Remove(exportableEvent);

                            yield return new BmsCommand
                            {
                                Name = $"{measure:D3}{laneGroup.Key}",
                                UseColon = true,
                                Value = exportedEvents
                            };
                        }
                    }
                }
            }
        }
    }
}