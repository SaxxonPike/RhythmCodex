using System;
using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Infrastructure
{
    internal static class EnumerableUtilities
    {
        public static IList<IList<T>> Deinterleave<T>(this IEnumerable<T> data, int interleave, int streamCount)
        {
            if (streamCount < 0)
                throw new RhythmCodexException($"{nameof(streamCount)} must be greater than or equal to zero.");
            if (streamCount == 0)
                return new List<IList<T>>();
            if (streamCount == 1)
                return new List<IList<T>> {data.ToList()};
            
            var result = Enumerable.Range(0, streamCount).Select(i => new List<T>()).Cast<IList<T>>().ToList();
            var index = 0;
            var channel = 0;
            
            foreach (var i in data)
            {
                result[channel].Add(i);
                
                index++;
                
                if (index >= interleave)
                {
                    index = 0;
                    channel++;
                }

                if (channel >= streamCount)
                    channel = 0;
            }

            return result;
        }
    }
}