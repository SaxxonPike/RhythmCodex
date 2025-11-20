using System;
using System.Diagnostics;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Extensions;

[DebuggerStepThrough]
internal static class ByteArrayExtensions
{
    public static void Swap16(this Span<byte> array)
    {
        if ((array.Length & 1) != 0)
            throw new RhythmCodexException("Array must have an even length in order to byte-swap 16-bit words.");

        for (var i = 0; i < array.Length; i += 2)
            (array[i], array[i + 1]) = (array[i + 1], array[i]);
    }
}