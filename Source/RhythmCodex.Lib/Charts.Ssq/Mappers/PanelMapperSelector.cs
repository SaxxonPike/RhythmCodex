using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Ssq.Converters;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Ssq.Mappers;

[Service]
public class PanelMapperSelector(
    IEnumerable<IPanelMapper> panelMappers,
    IStepPanelSplitter stepPanelSplitter,
    ILogger logger)
    : IPanelMapperSelector
{
    private IPanelMapper SelectInternal(int[] panelsUsed, int? playerCount, int? panelCount)
    {
        var eligibleMappers = panelMappers
            .Where(m => (playerCount == null || playerCount == m.PlayerCount) &&
                        m.ShouldMap(panelsUsed))
            .Distinct()
            .ToArray();

        return AssertMappers(eligibleMappers, playerCount, panelCount);
    }

    private IPanelMapper SelectInternal(PanelMapping[] panelsUsed, int? playerCount, int? panelCount)
    {
        var eligibleMappers = panelMappers
            .Where(m => (playerCount == null || playerCount == m.PlayerCount) &&
                        m.ShouldMap(panelsUsed))
            .Distinct()
            .ToArray();

        return AssertMappers(eligibleMappers, playerCount, panelCount);
    }

    private IPanelMapper AssertMappers(IPanelMapper[] eligibleMappers, int? playerCount, int? panelCount)
    {
        switch (eligibleMappers.Length)
        {
            case 0:
                logger.Warning($"No eligible mappers for {playerCount} player(s) and {panelCount} panel(s)");
                return null;
            case > 1:
            {
                var mapperNames = eligibleMappers.Select(m => m.GetType().Name).ToArray();
                logger.Debug($"Multiple eligible mappers for {playerCount} player(s) and {panelCount} panel(s)");
                logger.Debug($"Eligible mappers: {string.Join(", ", mapperNames)}");
                var chosenMapper = eligibleMappers.OrderBy(m => m.PanelCount).First();
                return chosenMapper;
            }
            default:
                return eligibleMappers.FirstOrDefault();
        }
    }

    public IPanelMapper Select(IEnumerable<Step> steps, ChartInfo chartInfo)
    {
        var panelsUsed = steps
            .Select(s => s.Panels | (s.ExtraPanels ?? 0))
            .SelectMany(stepPanelSplitter.Split)
            .Distinct()
            .ToArray();

        return SelectInternal(panelsUsed, chartInfo?.PlayerCount, chartInfo?.PanelCount);
    }

    public IPanelMapper Select(IEnumerable<Event> events, Metadata metadata)
    {
        var notes = events
            .Where(ev => ev[FlagData.Note] == true)
            .ToList();

        var panelsUsed = notes
            .Select(ev => new PanelMapping
            {
                Player = (int) (ev[NumericData.Player] ?? BigRational.Zero), 
                Panel = (int) (ev[NumericData.Column] ?? BigRational.Zero)
            })
            .ToArray();

        var playerCount = notes
            .Select(ev => (int) (ev[NumericData.Player] ?? 0))
            .Max() + 1;

        var panelCount = metadata[NumericData.ColumnCount] ??
                         panelsUsed.Select(p => p.Panel).Distinct().Count();

        return SelectInternal(panelsUsed, playerCount, panelCount == null ? null : (int) panelCount);
    }

    public IPanelMapper Select(int id)
    {
        var playerCount = (id >> 4) & 0xF;
        var panelCount = id & 0xF;
        var eligibleMappers = panelMappers.Where(pm => pm.PanelCount == (id & 0xF) &&
                                                        pm.PlayerCount == ((id >> 4) & 0xF))
            .Distinct()
            .ToArray();

        return AssertMappers(eligibleMappers, playerCount, panelCount);
    }
}