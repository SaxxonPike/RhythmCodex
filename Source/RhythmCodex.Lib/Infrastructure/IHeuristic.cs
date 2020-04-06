using System;
using System.IO;
using RhythmCodex.Heuristics;

namespace RhythmCodex.Infrastructure
{
    public interface IHeuristic
    {
        string Description { get; }
        string FileExtension { get; }
        HeuristicResult Match(ReadOnlySpan<byte> data);
        HeuristicResult Match(Stream stream);
        int MinimumLength { get; }
    }
}