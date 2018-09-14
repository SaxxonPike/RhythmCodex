using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Bms.Converters
{
    [Service]
    public class BmsEncoder : IBmsEncoder
    {
        private readonly IAlphabetConverter _alphabetConverter;
        private readonly ILogger _logger;

        public BmsEncoder(IAlphabetConverter alphabetConverter, ILogger logger)
        {
            _alphabetConverter = alphabetConverter;
            _logger = logger;
        }
        
        public IList<string> Encode(IChart chart)
        {
            return EncodeInternal(chart).ToList();
        }

        private IEnumerable<string> EncodeInternal(IChart chart)
        {
            if (chart.Events.Any(ev => ev[NumericData.MetricOffset] == null))
                throw new RhythmCodexException("Metric offsets must all be populated in order to export to BMS.");
            
            yield return $"; RhythmCodex {DateTime.Now:s}";

            var usedSamples = chart.Events
                .Where(ev => ev[NumericData.LoadSound] != null)
                .Select(ev => ev[NumericData.LoadSound])
                .Concat(chart.Events
                    .Where(ev => ev[NumericData.PlaySound] != null)
                    .Select(ev => ev[NumericData.PlaySound]))
                .Select(br => (int)br)
                .Distinct()
                .ToList();
            
            if (usedSamples.Count > 1295)
                _logger.Warning($"{nameof(BmsEncoder)}: there are more than 1295 samples - WAV list will be truncated. " +
                                $"{usedSamples.Count - 1295} samples will not be mapped.");

            var sampleMap = usedSamples
                .Select((e, i) => new KeyValuePair<int, int>(e, i + 1))
                .Where(kv => kv.Value > 0 && kv.Value <= 1295)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach (var kv in sampleMap)
            {
                yield return $"#WAV{_alphabetConverter.EncodeAlphanumeric(kv.Value, 2)} " +
                             $"{_alphabetConverter.EncodeAlphanumeric(kv.Key, 2)}.WAV";
            }

            var bpms = chart.Events
                .Where(ev => ev[NumericData.Bpm] != null)
                .Select(ev => (decimal) ev[NumericData.Bpm])
                .ToList();

            yield return $"#BPM {bpms[0]}";

            var bpmMap = bpms
                .Distinct()
                .Select((e, i) => new KeyValuePair<decimal, int>(e, i + 1))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach (var kv in bpmMap)
                yield return $"#BPM{_alphabetConverter.EncodeNumeric(kv.Value, 2)} {kv.Key}";
        }
    }
}