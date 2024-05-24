using System.IO;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Heuristics;

public interface IReadableHeuristic<out TOutput> : IHeuristic
{
    TOutput? Read(HeuristicResult heuristicResult, Stream stream);
}