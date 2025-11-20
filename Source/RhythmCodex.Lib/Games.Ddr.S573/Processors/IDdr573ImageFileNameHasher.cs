using System.Collections.Generic;

namespace RhythmCodex.Games.Ddr.S573.Processors;

public interface IDdr573ImageFileNameHasher
{
    int Calculate(string name);
    Dictionary<int, string> Reverse(params int[] hashes);
}