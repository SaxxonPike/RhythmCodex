using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Mappers;

[Service]
public class PanelMapperSelector(
    IEnumerable<IPanelMapper> panelMappers,
    IStepPanelSplitter stepPanelSplitter,
    ILogger logger)
    : IPanelMapperSelector
{
    public IPanelMapper Select(IEnumerable<Step> steps, ChartInfo chartInfo)
    {
        var panelsUsed = steps
            .Select(s => s.Panels | (s.ExtraPanels ?? 0))
            .SelectMany(stepPanelSplitter.Split)
            .Distinct()
            .ToArray();

        var eligibleMappers = panelMappers
            .Where(m => (chartInfo?.PlayerCount == null || chartInfo.PlayerCount == m.PlayerCount) && 
                        panelsUsed.Select(m.Map).All(p => p != null))
            .Distinct()
            .ToArray();

        if (eligibleMappers.Length == 0)
        {
            logger.Warning($"No eligible mappers for {chartInfo?.PlayerCount} player(s) and {chartInfo?.PanelCount} panel(s)");
        }
        else if (eligibleMappers.Length > 1)
        {
            var mapperNames = eligibleMappers.Select(m => m.GetType().Name).ToArray();
            logger.Debug($"Multiple eligible mappers for {chartInfo?.PlayerCount} player(s) and {chartInfo?.PanelCount} panel(s)");
            logger.Debug($"Eligible mappers: {string.Join(", ", mapperNames)}");
            var chosenMapper = eligibleMappers.OrderBy(m => m.PanelCount).First();
            return chosenMapper;
        }

        return eligibleMappers.FirstOrDefault();
    }
}