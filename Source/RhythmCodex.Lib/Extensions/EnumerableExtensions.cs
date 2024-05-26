using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Extensions;

[PublicAPI]
internal static class EnumerableExtensions
{
    [DebuggerStepThrough]
    public static IReadOnlyCollection<T> AsCollection<T>(this IEnumerable<T> enumerable) =>
        enumerable as IReadOnlyCollection<T> ?? enumerable.ToList();

    /// <summary>
    /// Interprets the object as a list, and creates one if it isn't already a list.
    /// </summary>
    [DebuggerStepThrough]
    public static IReadOnlyList<T> AsList<T>(this IEnumerable<T> enumerable) =>
        enumerable as IReadOnlyList<T> ?? enumerable.ToList();

    /// <summary>
    /// Interprets the object as an array, and creates one if it isn't already an array.
    /// </summary>
    [DebuggerStepThrough]
    public static T[] AsArray<T>(this IEnumerable<T> enumerable) =>
        enumerable as T[] ?? enumerable.ToArray();

    [DebuggerStepThrough]
    public static List<T[]> Deinterleave<T>(this Memory<T> data, int interleave, int streamCount) =>
        Deinterleave(data.Span, interleave, streamCount);

    [DebuggerStepThrough]
    public static List<T[]> Deinterleave<T>(this ReadOnlyMemory<T> data, int interleave, int streamCount) =>
        Deinterleave(data.Span, interleave, streamCount);

    [DebuggerStepThrough]
    public static List<T[]> Deinterleave<T>(this Span<T> data, int interleave, int streamCount) =>
        Deinterleave((ReadOnlySpan<T>)data, interleave, streamCount);

    [DebuggerStepThrough]
    public static List<T[]> Deinterleave<T>(this ReadOnlySpan<T> data, int interleave, int streamCount)
    {
        switch (streamCount)
        {
            case < 0:
                throw new RhythmCodexException($"{nameof(streamCount)} must be greater than or equal to zero.");
            case 0:
                return [];
            case 1:
                return [[..data]];
        }

        var result = new List<T[]>(streamCount);
        var blockSize = interleave * streamCount;
        var blockCount = data.Length / blockSize;
        var streamSize = blockCount * interleave;

        for (var channel = 0; channel < streamCount; channel++)
        {
            var target = new T[streamSize];
            var sourceCursor = data[(interleave * channel)..];
            var targetCursor = target.AsSpan();

            for (var block = 0; block < blockCount; block++)
            {
                sourceCursor[..interleave].CopyTo(targetCursor);
                if (sourceCursor.Length < blockSize)
                    break;
                sourceCursor = sourceCursor[blockSize..];
                targetCursor = targetCursor[interleave..];
            }

            result.Add(target);
        }

        return result;
    }
}