﻿using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Extensions
{
    public static class EnumerableExtensions
    {
        public static IList<T> AsList<T>(this IEnumerable<T> enumerable) => 
            enumerable as IList<T> ?? enumerable.ToList();

        public static T[] AsArray<T>(this IEnumerable<T> enumerable) => 
            enumerable as T[] ?? enumerable.ToArray();
    }
}
