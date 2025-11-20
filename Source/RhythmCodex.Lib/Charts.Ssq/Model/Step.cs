using RhythmCodex.Infrastructure;

namespace RhythmCodex.Charts.Ssq.Model;

[Model]
public record Step
{
    public int MetricOffset { get; set; }
    public byte Panels { get; set; }
    public byte? ExtraPanels { get; set; }
    public byte? ExtraPanelInfo { get; set; }
}