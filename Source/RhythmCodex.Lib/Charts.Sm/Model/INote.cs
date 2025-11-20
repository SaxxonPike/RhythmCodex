using RhythmCodex.Infrastructure;

namespace RhythmCodex.Charts.Sm.Model;

public interface INote
{
    int Column { get; }
    BigRational MetricOffset { get; }
    char Type { get; }
}