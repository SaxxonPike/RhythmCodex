using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Step1.Models;

namespace RhythmCodex.Step1.Converters;

public interface IStep1Decoder
{
    List<Chart> Decode(IEnumerable<Step1Chunk> data);
}