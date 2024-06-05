using System;
using System.Collections.Generic;

namespace ClientCommon;

public static class Iteration
{
    public static void ForEach<T>(IEnumerable<T> items, Action<T> action)
    {
        foreach (var item in items)
            action(item);
    }

    public static void ForEach<T>(IEnumerable<T> items, Action<T, int> action)
    {
        var idx = 0;
        foreach (var item in items)
            action(item, idx++);
    }
}