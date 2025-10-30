using System.Collections.Generic;
using JetBrains.Annotations;
using RhythmCodex.Charting.Models;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[PublicAPI]
public interface ISsqDecoder
{
    List<Chart> Decode(IEnumerable<SsqChunk> data);
}