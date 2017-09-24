using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class PanelMapperSelector : IPanelMapperSelector
    {
        private readonly IEnumerable<IPanelMapper> _panelMappers;
        private readonly IStepPanelSplitter _stepPanelSplitter;

        public PanelMapperSelector(IEnumerable<IPanelMapper> panelMappers, IStepPanelSplitter stepPanelSplitter)
        {
            _panelMappers = panelMappers;
            _stepPanelSplitter = stepPanelSplitter;
        }

        public IPanelMapper Select(IEnumerable<Step> steps, ChartInfo chartInfo)
        {
            var panelsUsed = steps
                .Select(s => s.Panels | (s.ExtraPanels ?? 0))
                .SelectMany(_stepPanelSplitter.Split)
                .Distinct()
                .ToArray();

            var eligibleMappers = _panelMappers
                .Where(m => chartInfo.PlayerCount <= m.PlayerCount && chartInfo.PanelCount <= m.PanelCount)
                .Where(m => panelsUsed.Select(m.Map).All(p => p != null))
                .ToArray();

            return eligibleMappers
                .Aggregate((a, b) => a.PanelCount < b.PanelCount ? a : b);
        }
    }
}