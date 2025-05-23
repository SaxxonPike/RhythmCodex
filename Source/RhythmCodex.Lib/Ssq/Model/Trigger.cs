using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ssq.Model;

[Model]
public record Trigger
{
    public short Id { get; set; }
    public int MetricOffset { get; set; }
}