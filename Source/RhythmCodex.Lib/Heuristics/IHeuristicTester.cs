using System;
using System.Collections.Generic;

namespace RhythmCodex.Heuristics
{
    public interface IHeuristicTester
    {
        IEnumerable<HeuristicResult> Match(ReadOnlySpan<byte> data, params Context[] contexts);
    }
}