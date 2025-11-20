using RhythmCodex.Infrastructure;

namespace RhythmCodex.Charts.Ssq.Model;

[Model]
public record Trigger
{
    public short Id { get; set; }
    public int MetricOffset { get; set; }
}