using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RhythmCodex.Infrastructure;

public static class MemoryCollectionExtensions
{
    public static Memory<T> Combine<T>(this IEnumerable<Memory<T>> enumerable)
    {
        var all = enumerable.ToArray();
        if (all.Length < 1)
            return Memory<T>.Empty;

        var size = all.Sum(x => x.Length);
        var result = new T[size];
        var cursor = result.AsMemory();

        foreach (var item in all)
        {
            item.CopyTo(cursor);
            cursor = cursor[item.Length..];
        }

        return result;
    }
}