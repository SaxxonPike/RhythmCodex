using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ssq.Model;

[Model]
public record Timing
{
    public int MetricOffset { get; set; }
    public int LinearOffset { get; set; }
}