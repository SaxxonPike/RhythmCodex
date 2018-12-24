using System.Collections.Generic;
using System.Linq;

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
    }
}