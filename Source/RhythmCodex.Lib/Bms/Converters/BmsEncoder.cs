using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Bms.Model;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Bms.Converters
{
    [Service]
    public class BmsEncoder : IBmsEncoder
    {
        private readonly ILogger _logger;

        public BmsEncoder(ILogger logger)
        {
            _logger = logger;
        }

        public IList<BmsCommand> Encode(IChart chart)
        {
            return EncodeInternal(chart).ToList();
        }

        private IEnumerable<BmsCommand> EncodeInternal(IChart chart)
        {
            if (chart.Events.Any(ev => ev[NumericData.MetricOffset] == null))
                throw new RhythmCodexException("Metric offsets must all be populated in order to export to BMS.");

            yield return new BmsCommand {Comment = $"RhythmCodex {DateTime.Now:s}"};

            // Sample definitions (WAVxx)

            var usedSamples = chart.Events
                .Where(ev => ev[NumericData.LoadSound] != null)
                .Select(ev => ev[NumericData.LoadSound])
                .Concat(chart.Events
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
                .Where(kv => kv.Value > 0 && kv.Value <= 1295)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach (var kv in sampleMap)
            {
                yield return new BmsCommand
                {
                    Name = $"WAV{Alphabet.EncodeAlphanumeric(kv.Value, 2)}",
                    Value = $"{Alphabet.EncodeAlphanumeric(kv.Key, 2)}.WAV"
                };
            }

            // BPM definitions (for BPM and xxx08:)

            var bpms = chart.Events
                .Where(ev => ev[NumericData.Bpm] != null)
                .Select(ev => (decimal) ev[NumericData.Bpm])
                .ToList();

            yield return new BmsCommand
            {
                Name = "BPM",
                Value = $"{bpms[0]}"
            };

            var bpmMap = bpms
                .Distinct()
                .Select((e, i) => new KeyValuePair<decimal, int>(e, i + 1))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach (var kv in bpmMap)
                yield return new BmsCommand
                {
                    Name = $"BPM{Alphabet.EncodeNumeric(kv.Value, 2)}",
                    Value = $"{kv.Key}"
                };

            // Measure lengths (xxx02:)

            var measureLengths = chart.Events
                .Where(ev => (ev[FlagData.Measure] == true || ev[FlagData.End] == true)
                             && ev[NumericData.MeasureLength].HasValue)
                .GroupBy(ev => (int) ev[NumericData.MetricOffset].Value.GetWholePart())
                .ToDictionary(
                    g => g.Key,
                    g => (decimal) (g.First()[NumericData.MeasureLength] ?? 1));

            foreach (var kv in measureLengths.Where(ml => ml.Value != 1))
            {
                yield return new BmsCommand
                {
                    Name = $"{Alphabet.EncodeNumeric(kv.Key, 3)}02",
                    Value = $"{kv.Value}",
                    UseColon = true
                };
            }
        }
    }
}