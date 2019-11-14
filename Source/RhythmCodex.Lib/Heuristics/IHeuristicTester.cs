using System;
using System.Collections.Generic;

namespace RhythmCodex.Heuristics
{
    public interface IHeuristicTester
    {
        IList<HeuristicResult> Match(ReadOnlySpan<byte> data, params Context[] contexts);
    }
}