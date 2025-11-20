using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Step1.Models;

namespace RhythmCodex.Charts.Step1.Converters;

public interface IStep1Decoder
{
    List<Chart> Decode(IEnumerable<Step1Chunk> data);
}