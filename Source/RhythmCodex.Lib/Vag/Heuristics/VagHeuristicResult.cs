using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Vag.Heuristics;

public class VagHeuristicResult(IHeuristic heuristic) : HeuristicResult(heuristic)
{
    public BigRational SampleRate { get; set; }
    public int? Channels { get; set; }
    public int? Interleave { get; set; }
    public int? Start { get; set; }
    public int? Length { get; set; }
    public BigRational? Volume { get; set; }
    public int? LoopStart { get; set; }
    public int? LoopEnd { get; set; }
    public byte[]? Key { get; set; }
}