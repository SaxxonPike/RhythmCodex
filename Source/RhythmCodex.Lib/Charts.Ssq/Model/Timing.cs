using RhythmCodex.Infrastructure;

namespace RhythmCodex.Charts.Ssq.Model;

[Model]
public record Timing
{
    public int MetricOffset { get; set; }
    public int LinearOffset { get; set; }
}