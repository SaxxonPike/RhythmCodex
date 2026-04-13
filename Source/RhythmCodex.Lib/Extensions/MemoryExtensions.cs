using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Extensions;

[PublicAPI]
public static class MemoryExtensions
{
    extension(ReadOnlyMemory<byte> data)
    {
        /// <summary>
        /// Convert a string from bytes using Codepage 437.
        /// </summary>
        public string GetString()
            => Encodings.Cp437.GetString(data.Span);
    }

    extension(Memory<byte> data)
    {
        /// <summary>
        /// Convert a string from bytes using Codepage 437.
        /// </summary>
        public string GetString()
            => Encodings.Cp437.GetString(data.Span);
    }

    extension<T>(Memory<T> data)
    {
        [DebuggerStepThrough]
        public List<T[]> Deinterleave(int interleave, int streamCount) =>
            data.Span.Deinterleave(interleave, streamCount);
    }

    extension<T>(ReadOnlyMemory<T> data)
    {
        [DebuggerStepThrough]
        public List<T[]> Deinterleave(int interleave, int streamCount) =>
            data.Span.Deinterleave(interleave, streamCount);
    }
}