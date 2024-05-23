using System;
using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Heuristics;

public interface IHeuristicTester
{
    IList<HeuristicResult> Match(Stream stream, long length, params Context[] contexts);
    IList<HeuristicResult> Match(Memory<byte> data, params Context[] contexts);
}