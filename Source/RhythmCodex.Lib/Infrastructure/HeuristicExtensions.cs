using System;
using System.IO;
using RhythmCodex.Heuristics;

namespace RhythmCodex.Infrastructure;

public static class HeuristicExtensions
{
    extension(IHeuristic heuristic)
    {
        public HeuristicResult? Match(Stream stream) =>
            heuristic.Match(new StreamHeuristicReader(stream));

        public HeuristicResult? Match(ReadOnlyMemory<byte> data) =>
            heuristic.Match(new MemoryHeuristicReader(data));
    }
}