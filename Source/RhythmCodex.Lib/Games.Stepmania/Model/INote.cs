using RhythmCodex.Infrastructure;

namespace RhythmCodex.Games.Stepmania.Model;

public interface INote
{
    int Column { get; }
    BigRational MetricOffset { get; }
    char Type { get; }
}