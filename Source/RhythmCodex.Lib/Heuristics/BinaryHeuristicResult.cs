using RhythmCodex.Infrastructure;

namespace RhythmCodex.Heuristics;

public class BinaryHeuristicResult(IHeuristic heuristic) : HeuristicResult(heuristic)
{
    public long Offset { get; set; }
    public long Length { get; set; }
}