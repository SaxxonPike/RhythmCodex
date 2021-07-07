using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ssq.Model
{
    [Model]
    public class Timing
    {
        public int MetricOffset { get; init; }
        public int LinearOffset { get; init; }
    }
}