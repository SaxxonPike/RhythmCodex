using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Interprets the object as a list, and creates one if it isn't already a list.
        /// </summary>
        public static IList<T> AsList<T>(this IEnumerable<T> enumerable) => 
            enumerable as IList<T> ?? enumerable.ToList();

        /// <summary>
        /// Interprets the object as an array, and creates one if it isn't already an array.
        /// </summary>
        public static T[] AsArray<T>(this IEnumerable<T> enumerable) => 
            enumerable as T[] ?? enumerable.ToArray();
        
        public static IList<IList<T>> Deinterleave<T>(this IEnumerable<T> data, int interleave, int streamCount)
        {
            if (streamCount < 0)
                throw new RhythmCodexException($"{nameof(streamCount)} must be greater than or equal to zero.");
            if (streamCount == 0)
                return new List<IList<T>>();
            if (streamCount == 1)
                return new List<IList<T>> {data.ToList()};
            
            var result = Enumerable.Range(0, streamCount).Select(_ => new List<T>()).Cast<IList<T>>().ToList();
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