using RhythmCodex.Infrastructure;

namespace RhythmCodex.Archs.Djmain.Model;

[Model]
public record DjmainDecodeOptions
{
    public bool DisableAudio { get; init; }
    public bool DoNotConsolidateSamples { get; init; }
    public bool SwapStereo { get; init; } = true;
    public DjmainChartType ChartType { get; init; } = DjmainChartType.Beatmania;
    public DjmainChartTiming ChartTiming { get; init; } = DjmainChartTiming.Arcade;
}