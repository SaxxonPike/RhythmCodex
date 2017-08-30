using Numerics;

namespace RhythmCodex.Stepmania.Model
{
    public class Note : INote
    {
        public BigRational MetricOffset { get; set; }
        public int Column { get; set; }
        public char Type { get; set; }
    }
}
