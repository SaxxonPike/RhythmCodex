using System.Collections.Generic;

namespace RhythmCodex.Infrastructure.Converters
{
    public interface IDeinterleaver
    {
        IList<IList<T>> Deinterleave<T>(IEnumerable<T> data, int interleave, int streamCount);
    }
}