using RhythmCodex.Infrastructure;

namespace RhythmCodex.Heuristics;

[Model]
public class HeuristicResult(IHeuristic heuristic)
{
    public IHeuristic Heuristic { get; } = heuristic;
}