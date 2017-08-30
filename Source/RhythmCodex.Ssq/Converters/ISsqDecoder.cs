using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ISsqDecoder
    {
        IEnumerable<IChart> Decode(IEnumerable<IChunk> data);
    }
}