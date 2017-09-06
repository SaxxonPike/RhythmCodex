using Numerics;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Stepmania.Model
{
    [Model]
    public class Note : INote
    {
        public BigRational MetricOffset { get; set; }
        public int Column { get; set; }
        public char Type { get; set; }
    }
}
