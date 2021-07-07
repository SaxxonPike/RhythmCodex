using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ssq.Model
{
    [Model]
    public class Trigger
    {
        public short Id { get; init; }
        public int MetricOffset { get; init; }
    }
}