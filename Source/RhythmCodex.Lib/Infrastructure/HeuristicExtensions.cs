using System;
using System.IO;
using RhythmCodex.Heuristics;

namespace RhythmCodex.Infrastructure;

public static class HeuristicExtensions
{
    public static HeuristicResult? Match(this IHeuristic heuristic, Stream stream) =>
        heuristic.Match(new StreamHeuristicReader(stream));

    public static HeuristicResult? Match(this IHeuristic heuristic, ReadOnlyMemory<byte> data) =>
        heuristic.Match(new MemoryHeuristicReader(data));
}