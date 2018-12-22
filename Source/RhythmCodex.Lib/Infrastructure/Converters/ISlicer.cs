using System;

namespace RhythmCodex.Infrastructure.Converters
{
    public interface ISlicer
    {
        ArraySegment<T> Slice<T>(T[] arr, int offset, int length);
    }
}