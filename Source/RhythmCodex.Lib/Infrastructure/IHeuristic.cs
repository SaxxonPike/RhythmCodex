using System;
using RhythmCodex.Heuristics;

namespace RhythmCodex.Infrastructure
{
    public interface IHeuristic
    {
        string Description { get; }
        string FileExtension { get; }
        HeuristicResult Match(ReadOnlySpan<byte> data);
        int MinimumLength { get; }
    }
}