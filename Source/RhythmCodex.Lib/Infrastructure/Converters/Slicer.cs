using System;

namespace RhythmCodex.Infrastructure.Converters
{
    [Service]
    public class Slicer : ISlicer
    {
        public T[] Slice<T>(T[] arr, int offset, int length)
        {
            var result = new T[length];
            Array.Copy(arr, offset, result, 0, length);
            return result;
        }
    }
}