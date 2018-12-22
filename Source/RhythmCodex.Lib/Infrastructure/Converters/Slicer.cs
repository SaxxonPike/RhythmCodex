using System;

namespace RhythmCodex.Infrastructure.Converters
{
    [Service]
    public class Slicer : ISlicer
    {
        public ArraySegment<T> Slice<T>(T[] arr, int offset, int length)
        {
            return new ArraySegment<T>(arr, offset, length);
        }
    }
}