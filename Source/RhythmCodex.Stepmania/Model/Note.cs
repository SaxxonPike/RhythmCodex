using Numerics;

namespace RhythmCodex.Stepmania.Model
{
    public struct Note
    {
        public BigRational MetricOffset { get; set; }
        public int Column { get; set; }
        public char Type { get; set; }
    }
}
