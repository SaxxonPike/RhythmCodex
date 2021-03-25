using System;
using System.Collections.Generic;

namespace ClientCommon
{
    public static class Iteration
    {
        public static void ForEach<T>(IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
                action(item);
        }
    }
}