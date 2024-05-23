using System.Collections.Generic;

namespace RhythmCodex.Ddr.Processors;

public interface IDdr573ImageFileNameHasher
{
    int Calculate(string name);
    IDictionary<int, string> Reverse(params int[] hashes);
}