using System;
using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Heuristics;

public interface IHeuristicTester
{
    List<HeuristicResult> Match(Stream stream, long length, params Context[] contexts);
    List<HeuristicResult> Match(Memory<byte> data, params Context[] contexts);
}