using System;

namespace RhythmCodex.Infrastructure;

/// <summary>
/// Manages a block of read-only, zero-initialized memory.
/// </summary>
public static class ZeroMemory
{
    private static byte[] _memory = new byte[4096];

    private static byte[] GetInstance(int size)
    {
        if (_memory.Length >= size)
            return _memory;

        var newLength = _memory.Length;

        while (newLength < size)
            newLength <<= 1;

        _memory = new byte[newLength];

        return _memory;
    }

    /// <summary>
    /// Gets a read-only, zero-initialized memory block of the specified size.
    /// </summary>
    public static ReadOnlyMemory<byte> MemoryInstance(int size) =>
        GetInstance(size).AsMemory(0, size);

    /// <summary>
    /// Gets a read-only, zero-initialized memory block of the specified size.
    /// </summary>
    public static ReadOnlySpan<byte> SpanInstance(int size) =>
        GetInstance(size).AsSpan(0, size);
}