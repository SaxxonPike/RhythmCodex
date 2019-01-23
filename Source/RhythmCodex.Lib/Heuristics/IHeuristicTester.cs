using System;
using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Heuristics
{
    public interface IHeuristicTester
    {
        IEnumerable<IHeuristic> Find(ReadOnlySpan<byte> data);
    }
}