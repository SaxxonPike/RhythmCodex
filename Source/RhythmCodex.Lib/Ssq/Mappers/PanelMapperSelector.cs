using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Mappers
{
    [Service]
    public class PanelMapperSelector : IPanelMapperSelector
    {
        private readonly IEnumerable<IPanelMapper> _panelMappers;
        private readonly IStepPanelSplitter _stepPanelSplitter;
        private readonly ILogger _logger;

        public PanelMapperSelector(
            IEnumerable<IPanelMapper> panelMappers,
            IStepPanelSplitter stepPanelSplitter,
            ILogger logger)
        {
            _panelMappers = panelMappers;
            _stepPanelSplitter = stepPanelSplitter;
            _logger = logger;
        }

        public IPanelMapper Select(IEnumerable<Step> steps, ChartInfo chartInfo)
        {
            var panelsUsed = steps
                .Select(s => s.Panels | (s.ExtraPanels ?? 0))
                .SelectMany(_stepPanelSplitter.Split)
                .Distinct()
                .ToArray();

            var eligibleMappers = _panelMappers
                .Where(m => (chartInfo?.PlayerCount == null || chartInfo.PlayerCount == m.PlayerCount) && (chartInfo?.PanelCount == null || chartInfo.PanelCount == m.PanelCount))
                .Where(m => panelsUsed.Select(m.Map).All(p => p != null))
                .ToArray();

            if (eligibleMappers.Length == 0)
            {
                _logger.Warning($"No eligible mappers for {chartInfo?.PlayerCount} player(s) and {chartInfo?.PanelCount} panel(s)");
            }
            else if (eligibleMappers.Length > 1)
            {
                var mapperNames = eligibleMappers.Select(m => m.GetType().Name).ToArray();
                _logger.Warning($"Multiple eligible mappers for {chartInfo?.PlayerCount} player(s) and {chartInfo?.PanelCount} panel(s)");
                _logger.Debug($"Eligible mappers: {string.Join(", ", mapperNames)}");
            }

            return eligibleMappers.FirstOrDefault();
        }
    }
}