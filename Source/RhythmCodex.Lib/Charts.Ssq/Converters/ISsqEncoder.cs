using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Ssq.Converters
{
    public interface ISsqEncoder
    {
        IList<SsqChunk> Encode(IEnumerable<Chart> charts);
    }
}