namespace RhythmCodex.Heuristics;

public record HeuristicBlockResult
{
    public HeuristicResult? Result { get; init; }

    public int BlockIndex { get; init; }

    public long Offset { get; init; }
}