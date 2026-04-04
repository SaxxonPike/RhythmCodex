using System;
using System.Runtime.CompilerServices;

// ReSharper disable CheckNamespace

namespace Org.BouncyCastle.Utilities;

internal static class Spans
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Span<T> FromNullable<T>(T[]? array, int start)
    {
        return array == null ? Span<T>.Empty : array.AsSpan(start);
    }
}