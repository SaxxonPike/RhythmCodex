using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Mappers;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class SsqDecoder : ISsqDecoder
    {
        private readonly IChartInfoDecoder _chartInfoDecoder;
        private readonly IPanelMapperSelector _panelMapperSelector;
        private readonly ISsqChunkFilter _ssqChunkFilter;
        private readonly ISsqEventDecoder _ssqEventDecoder;
        private readonly IStepChunkDecoder _stepChunkDecoder;
        private readonly ITimingChunkDecoder _timingChunkDecoder;
        private readonly ITriggerChunkDecoder _triggerChunkDecoder;

        public SsqDecoder(
            ISsqEventDecoder ssqEventDecoder,
            ITimingChunkDecoder timingDecoder,
            IStepChunkDecoder stepChunkDecoder,
            ITriggerChunkDecoder triggerChunkDecoder,
            IPanelMapperSelector panelMapperSelector,
            IChartInfoDecoder chartInfoDecoder,
            ISsqChunkFilter ssqChunkFilter)
        {
            _ssqEventDecoder = ssqEventDecoder;
            _timingChunkDecoder = timingDecoder;
            _stepChunkDecoder = stepChunkDecoder;
            _triggerChunkDecoder = triggerChunkDecoder;
            _panelMapperSelector = panelMapperSelector;
            _chartInfoDecoder = chartInfoDecoder;
            _ssqChunkFilter = ssqChunkFilter;
        }

        public IList<IChart> Decode(IEnumerable<Chunk> data)
        {
            var chunks = data.AsList();

            var timings = _ssqChunkFilter.GetTimings(chunks);
            var triggers = _ssqChunkFilter.GetTriggers(chunks);
            var steps = _ssqChunkFilter.GetSteps(chunks);

            var charts = steps.Select(sc =>
            {
                var info = _chartInfoDecoder.Decode(sc.Id);

                return new Chart
                {
                    Events = _ssqEventDecoder.Decode(
                            timings,
                            sc.Steps,
                            triggers,
                            _panelMapperSelector.Select(sc.Steps, info))
                        .AsList(),
                    [NumericData.Id] = sc.Id,
                    [StringData.Difficulty] = info.Difficulty,
                    [StringData.Type] = $"dance-{info.Type.ToLowerInvariant()}"
                };
            });

            return charts.Cast<IChart>().AsList();
        }
    }
}