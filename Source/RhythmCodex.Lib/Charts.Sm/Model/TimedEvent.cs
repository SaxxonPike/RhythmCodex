using RhythmCodex.Infrastructure;

namespace RhythmCodex.Charts.Sm.Model;

[Model]
public class TimedEvent
{
    public BigRational Offset { get; set; }
    public BigRational Value { get; set; }
}