using RhythmCodex.Infrastructure;

namespace RhythmCodex.Stepmania.Model
{
    [Model]
    public class TimedEvent
    {
        public BigRational Offset { get; set; }
        public BigRational Value { get; set; }
    }
}