using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ssq.Model
{
    [Model]
    public class Trigger
    {
        public short Id { get; set; }
        public int MetricOffset { get; set; }
    }
}
