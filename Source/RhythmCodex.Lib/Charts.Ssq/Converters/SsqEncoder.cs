using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charts.Filters;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Ssq.Mappers;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Ssq.Converters
{
    [Service]
    public class SsqEncoder : ISsqEncoder
    {
        private readonly IChartEventFilter _chartEventFilter;
        private readonly ITimingEventEncoder _timingEventEncoder;
        private readonly IStepChunkEncoder _stepChunkEncoder;
        private readonly IStepEventEncoder _stepEventEncoder;
        private readonly ITriggerChunkEncoder _triggerChunkEncoder;
        private readonly ITimingChunkEncoder _timingChunkEncoder;
        private readonly IPanelMapperSelector _panelMapperSelector;
        private readonly ISsqIdSelector _ssqIdSelector;
        private readonly ILogger _logger;
        private readonly ITriggerEventEncoder _triggerEventEncoder;

        public SsqEncoder(IChartEventFilter chartEventFilter, ITimingEventEncoder timingEventEncoder,
            ITriggerEventEncoder triggerEventEncoder, IStepChunkEncoder stepChunkEncoder,
            IStepEventEncoder stepEventEncoder, ITriggerChunkEncoder triggerChunkEncoder,
            ITimingChunkEncoder timingChunkEncoder, IPanelMapperSelector panelMapperSelector,
            ISsqIdSelector ssqIdSelector, ILogger logger)
        {
            _chartEventFilter = chartEventFilter;
            _timingEventEncoder = timingEventEncoder;
            _triggerEventEncoder = triggerEventEncoder;
            _stepChunkEncoder = stepChunkEncoder;
            _stepEventEncoder = stepEventEncoder;
            _triggerChunkEncoder = triggerChunkEncoder;
            _timingChunkEncoder = timingChunkEncoder;
            _panelMapperSelector = panelMapperSelector;
            _ssqIdSelector = ssqIdSelector;
            _logger = logger;
        }

        public IList<SsqChunk> Encode(IEnumerable<Chart> charts)
        {
            var chartList = charts.AsList();
            var result = new List<SsqChunk>();

            var maxLength = chartList
                .Select(c => c.GetMetricLength())
                .Max();
            if (maxLength < 1)
                maxLength = 1;

            var timingOffset = chartList
                                   .Select(c => c[NumericData.LinearOffset])
                                   .FirstOrDefault(x => x != null)
                               ?? 0;

            var bpms = chartList
                .Select(c => _chartEventFilter.GetBpms(c.Events))
                .FirstOrDefault(x => x.Any(b => b[NumericData.Bpm] != null));

            var initialBpm = chartList
                .Select(c => c[NumericData.Bpm])
                .FirstOrDefault(x => x != null);

            if (bpms == null || !bpms.Any())
                throw new RhythmCodexException("No BPMs found.");

            var rate = chartList
                .Select(c => c[NumericData.Rate] ?? c[NumericData.SourceRate])
                .FirstOrDefault(r => r != null) ?? SsqConstants.DefaultRate;

            var timings = _timingEventEncoder.Encode(bpms, (int) rate, maxLength, timingOffset, initialBpm);

            var triggers = chartList
                .Select(c => _chartEventFilter.GetTriggers(c.Events))
                .FirstOrDefault(x => x.Any());

            if (triggers == null)
            {
                // no triggers is fine, we will generate our own
                triggers = new List<Event>
                {
                    new()
                    {
                        [NumericData.MetricOffset] = BigRational.Zero,
                        [NumericData.Trigger] = 0x0401
                    },
                    new()
                    {
                        [NumericData.MetricOffset] = BigRational.Zero,
                        [NumericData.Trigger] = 0x0102
                    },
                    new()
                    {
                        [NumericData.MetricOffset] = SsqConstants.MeasureLength,
                        [NumericData.Trigger] = 0x0202
                    },
                    new()
                    {
                        [NumericData.MetricOffset] = SsqConstants.MeasureLength,
                        [NumericData.Trigger] = 0x0502
                    },
                    new()
                    {
                        [NumericData.MetricOffset] = maxLength - SsqConstants.MeasureLength * 3 / 4,
                        [NumericData.Trigger] = 0x0302
                    },
                    new()
                    {
                        [NumericData.MetricOffset] = maxLength,
                        [NumericData.Trigger] = 0x0402
                    },
                };
            }

            var timingData = _timingChunkEncoder.Convert(timings.Timings);
            var timingChunk = new SsqChunk
            {
                Parameter0 = SsqConstants.Parameter0.Timings,
                Parameter1 = (short) timings.Rate,
                Data = timingData
            };
            result.Add(timingChunk);

            var triggerData = _triggerChunkEncoder.Convert(_triggerEventEncoder.Encode(triggers));
            var triggerChunk = new SsqChunk
            {
                Parameter0 = SsqConstants.Parameter0.Triggers,
                Parameter1 = 1,
                Data = triggerData
            };
            result.Add(triggerChunk);

            foreach (var chart in chartList)
            {
                // Need to determine the chart type/id before we can map any panels.
                var difficultyId = _ssqIdSelector.SelectDifficulty(chart);
                var typeId = _ssqIdSelector.SelectType(chart);
                var chartId = chart[NumericData.Id];
                if (chartId == null && difficultyId != null && typeId != null)
                    chartId = difficultyId | typeId;

                // Try to infer what type the chart is if all we have is a difficulty.
                var mapper = typeId == null
                    ? _panelMapperSelector.Select(chart.Events, chart)
                    : _panelMapperSelector.Select((int) typeId);

                // We really ought to have a chart ID by now even if all we were given was a difficulty.
                if (chartId == null && difficultyId != null)
                    chartId = difficultyId | (mapper.PanelCount & 0xF) | ((mapper.PlayerCount & 0xF) << 4);
                if (chartId == null)
                {
                    _logger.Warning("Chart ID could not be determined. It will not be written to the SSQ.");
                    continue;
                }

                var steps = _chartEventFilter.GetNotes(chart.Events);
                var encodedSteps = _stepEventEncoder.Encode(steps, mapper);
                var stepData = _stepChunkEncoder.Convert(encodedSteps);
                var stepChunk = new SsqChunk
                {
                    Parameter0 = SsqConstants.Parameter0.Steps,
                    Parameter1 = (short) chartId,
                    Data = stepData
                };
                result.Add(stepChunk);
            }

            return result;
        }
    }
}