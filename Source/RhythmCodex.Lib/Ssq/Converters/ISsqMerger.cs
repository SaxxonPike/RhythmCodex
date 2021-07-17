using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public interface ISsqMerger
    {
        IList<SsqChunk> Merge(IEnumerable<SsqChunk> target, IEnumerable<SsqChunk> items);
    }
}