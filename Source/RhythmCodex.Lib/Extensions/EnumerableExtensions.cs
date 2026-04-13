using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace RhythmCodex.Extensions;

[PublicAPI]
internal static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> enumerable)
    {
        [DebuggerStepThrough]
        public IReadOnlyCollection<T> AsCollection() =>
            enumerable as IReadOnlyCollection<T> ?? enumerable.ToList();

        /// <summary>
        /// Interprets the object as a list, and creates one if it isn't already a list.
        /// </summary>
        [DebuggerStepThrough]
        public IReadOnlyList<T> AsList() =>
            enumerable as IReadOnlyList<T> ?? enumerable.ToList();

        /// <summary>
        /// Interprets the object as an array, and creates one if it isn't already an array.
        /// </summary>
        [DebuggerStepThrough]
        public T[] AsArray() =>
            enumerable as T[] ?? enumerable.ToArray();
    }
}