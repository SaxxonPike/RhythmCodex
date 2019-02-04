using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Vag.Heuristics
{
    public class VagHeuristicResult : HeuristicResult
    {
        public VagHeuristicResult(IHeuristic heuristic) : base(heuristic)
        {
        }
        
        public BigRational SampleRate { get; set; }
        public int? Channels { get; set; }
        public int? Interval { get; set; }
        public int? Start { get; set; }
        public int? Length { get; set; }
    }
}