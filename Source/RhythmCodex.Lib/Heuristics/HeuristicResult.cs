using RhythmCodex.Infrastructure;

namespace RhythmCodex.Heuristics
{
    [Model]
    public class HeuristicResult
    {
        public HeuristicResult(IHeuristic heuristic)
        {
            Heuristic = heuristic;
        }
        
        public IHeuristic Heuristic { get; }
    }
}