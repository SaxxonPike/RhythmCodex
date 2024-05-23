using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Heuristics;

public interface IHeuristicBlockStreamReader
{
    IEnumerable<HeuristicBlockResult> Find(Stream stream, long length, int blockSize, params Context[] contexts);
}