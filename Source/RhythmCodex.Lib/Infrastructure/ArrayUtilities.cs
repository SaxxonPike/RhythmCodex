using System;

namespace RhythmCodex.Infrastructure
{
    public static class ArrayUtilities
    {
        public static T[] Slice<T>(this T[] arr, int offset, int length) => arr.AsSpan(offset, length).ToArray();
    }
}