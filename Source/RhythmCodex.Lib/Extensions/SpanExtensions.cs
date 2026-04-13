using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Extensions;

[PublicAPI]
public static class SpanExtensions
{
    extension(ReadOnlySpan<byte> b)
    {
        /// <summary>
        /// Convert a string from bytes using Codepage 932.
        /// </summary>
        public string GetShiftJisString()
            => Encodings.Cp932.GetString(b);

        /// <summary>
        /// Convert a string from bytes using Codepage 437.
        /// </summary>
        public string GetString()
            => Encodings.Cp437.GetString(b);

        public ReadOnlySpan<byte> NullTerminated()
        {
            var idx = b.IndexOf((byte)0);
            return b[..(idx >= 0 ? idx : b.Length)];
        }
    }

    extension<T>(Span<T> data)
    {
        [DebuggerStepThrough]
        public List<T[]> Deinterleave(int interleave, int streamCount) =>
            ((ReadOnlySpan<T>)data).Deinterleave(interleave, streamCount);
    }

    extension<T>(ReadOnlySpan<T> data)
    {
        [DebuggerStepThrough]
        public List<T[]> Deinterleave(int interleave, int streamCount)
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
}