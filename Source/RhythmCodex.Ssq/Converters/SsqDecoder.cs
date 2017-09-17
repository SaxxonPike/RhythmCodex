using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class SsqDecoder : ISsqDecoder
    {
        private readonly ISsqEventDecoder _ssqEventDecoder;
        private readonly ITimingChunkDecoder _timingChunkDecoder;
        private readonly IStepChunkDecoder _stepChunkDecoder;
        private readonly ITriggerChunkDecoder _triggerChunkDecoder;
        private readonly IPanelMapperSelector _panelMapperSelector;
        private readonly IChartInfoDecoder _chartInfoDecoder;

        public SsqDecoder(
            ISsqEventDecoder ssqEventDecoder,
            ITimingChunkDecoder timingDecoder,
            IStepChunkDecoder stepChunkDecoder,
            ITriggerChunkDecoder triggerChunkDecoder,
            IPanelMapperSelector panelMapperSelector,
            IChartInfoDecoder chartInfoDecoder)
        {
            _ssqEventDecoder = ssqEventDecoder;
            _timingChunkDecoder = timingDecoder;
            _stepChunkDecoder = stepChunkDecoder;
            _triggerChunkDecoder = triggerChunkDecoder;
            _panelMapperSelector = panelMapperSelector;
            _chartInfoDecoder = chartInfoDecoder;
        }

        public IList<IChart> Decode(IEnumerable<IChunk> data)
        {
            var chunks = data.AsList();
            var timings = chunks.Where(c => c.Parameter0 == Parameter0.Timings)
                .SelectMany(tc => _timingChunkDecoder.Convert(tc.Data))
                .AsList();
            var triggers = chunks.Where(c => c.Parameter0 == Parameter0.Triggers)
                .SelectMany(tc => _triggerChunkDecoder.Convert(tc.Data))
                .AsList();
            var stepChunks = chunks.Where(c => c.Parameter0 == Parameter0.Steps)
                .AsList();
            var timingRate = chunks.First(c => c.Parameter0 == Parameter0.Timings).Parameter1;

            var charts = stepChunks.Select(sc =>
            {
                var steps = _stepChunkDecoder.Convert(sc.Data).AsList();
                var info = _chartInfoDecoder.Decode(sc.Parameter1);
                
                return new Chart
                {
                    Events = _ssqEventDecoder.Decode(
                        timings,
                        steps,
                        triggers,
                        _panelMapperSelector.Select(steps, info),
                        timingRate).AsList(),
                    [NumericData.Id] = sc.Parameter1,
                    ["Difficulty"] = info.Difficulty,
                    ["Type"] = info.Type
                };
            });

            return charts.Cast<IChart>().AsList();
        }
    }
}