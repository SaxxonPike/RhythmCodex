using System;
using System.Runtime.Intrinsics;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Sounds.Converters;

internal static class AudioSimd
{
    /// <summary>
    /// Quantizes Sound channels to an interleaved 16-bit audio data block.
    /// </summary>
    /// <param name="source">
    /// Data to be quantized.
    /// </param>
    /// <param name="target">
    /// Target buffer.
    /// </param>
    /// <param name="startSample">
    /// Offset within the target buffer to start writing data.
    /// </param>
    /// <param name="advanceSamples">
    /// Offset increment per value in the target buffer.
    /// </param>
    public static void Quantize16(
        ReadOnlySpan<float> source,
        Span<short> target,
        int startSample, 
        int advanceSamples
    )
    {
        if (Vector512.IsHardwareAccelerated)
        {
            var minusOne = Vector512.Create<float>(-1f);

            while (source.Length >= 16)
            {
                var v = Vector512.ConvertToInt32(
                    Vector512.Max(
                        minusOne,
                        Vector512.Min(
                            Vector512<float>.One,
                            Vector512.Create(source)
                        )
                    ) * 32767f
                );

                for (var i = 0; i < 16; i++)
                {
                    target[startSample] = unchecked((short)v[i]);
                    target = target[advanceSamples..];
                }
                
                source = source[16..];
            }
        }
        else if (Vector256.IsHardwareAccelerated)
        {
            var minusOne = Vector256.Create<float>(-1f);

            while (source.Length >= 8)
            {
                var v = Vector256.ConvertToInt32(
                    Vector256.Max(
                        minusOne,
                        Vector256.Min(
                            Vector256<float>.One,
                            Vector256.Create(source)
                        )
                    ) * 32767f
                );

                for (var i = 0; i < 8; i++)
                {
                    target[startSample] = unchecked((short)v[i]);
                    target = target[advanceSamples..];
                }
                
                source = source[8..];
            }
        }
        else if (Vector128.IsHardwareAccelerated)
        {
            var minusOne = Vector128.Create<float>(-1f);

            while (source.Length >= 4)
            {
                var v = Vector128.ConvertToInt32(
                    Vector128.Max(
                        minusOne,
                        Vector128.Min(
                            Vector128<float>.One,
                            Vector128.Create(source)
                        )
                    ) * 32767f
                );

                for (var i = 0; i < 4; i++)
                {
                    target[startSample] = unchecked((short)v[i]);
                    target = target[advanceSamples..];
                }
                
                source = source[4..];
            }
        }

        for (int i = 0, j = startSample; i < source.Length; i++, j += advanceSamples)
            target[j] = unchecked((short)(Math.Clamp(source[i], -1f, 1f) * 32767f));
    }

    public static void Mix(Span<float> target, ReadOnlySpan<float> source)
    {
        var targetCursor = target;
        var sourceCursor = source;
        var maxLength = Math.Min(target.Length, source.Length);

        if (Vector512.IsHardwareAccelerated)
        {
            while (maxLength >= 16)
            {
                var v = Vector512.Create<float>(targetCursor);
                v += Vector512.Create(sourceCursor);
                v.CopyTo(targetCursor);
                targetCursor = targetCursor[16..];
                sourceCursor = sourceCursor[16..];
                maxLength -= 16;
            }
        }
        else if (Vector256.IsHardwareAccelerated)
        {
            while (maxLength >= 8)
            {
                var v = Vector256.Create<float>(targetCursor);
                v += Vector256.Create(sourceCursor);
                v.CopyTo(targetCursor);
                targetCursor = targetCursor[8..];
                sourceCursor = sourceCursor[8..];
                maxLength -= 8;
            }
        }
        else if (Vector128.IsHardwareAccelerated)
        {
            while (maxLength >= 4)
            {
                var v = Vector128.Create<float>(targetCursor);
                v += Vector128.Create(sourceCursor);
                v.CopyTo(targetCursor);
                targetCursor = targetCursor[4..];
                sourceCursor = sourceCursor[4..];
                maxLength -= 4;
            }
        }
        else if (Vector64.IsHardwareAccelerated)
        {
            while (maxLength >= 2)
            {
                var v = Vector64.Create<float>(targetCursor);
                v += Vector64.Create(sourceCursor);
                v.CopyTo(targetCursor);
                targetCursor = targetCursor[2..];
                sourceCursor = sourceCursor[2..];
                maxLength -= 2;
            }
        }

        for (var i = 0; i < maxLength; i++)
            targetCursor[i] += sourceCursor[i];
    }

    public static void Gain(Span<float> data, float amp)
    {
        if (amp == 1)
            return;

        var cursor = data;

        if (Vector512.IsHardwareAccelerated)
        {
            while (cursor.Length >= 16)
            {
                var v = Vector512.Create<float>(cursor) * amp;
                v.CopyTo(cursor);
                cursor = cursor[16..];
            }
        }
        else if (Vector256.IsHardwareAccelerated)
        {
            while (cursor.Length >= 8)
            {
                var v = Vector256.Create<float>(cursor) * amp;
                v.CopyTo(cursor);
                cursor = cursor[8..];
            }
        }
        else if (Vector128.IsHardwareAccelerated)
        {
            while (cursor.Length >= 4)
            {
                var v = Vector128.Create<float>(cursor) * amp;
                v.CopyTo(cursor);
                cursor = cursor[4..];
            }
        }
        else if (Vector64.IsHardwareAccelerated)
        {
            while (cursor.Length >= 2)
            {
                var v = Vector64.Create<float>(cursor) * amp;
                v.CopyTo(cursor);
                cursor = cursor[2..];
            }
        }

        for (var i = 0; i < cursor.Length; i++)
            cursor[i] *= amp;
    }

    public static void Deinterleave2(ReadOnlySpan<float> data, Span<float> target0, Span<float> target1)
    {
        var inCursor = data;
        var outCursor0 = target0;
        var outCursor1 = target1;

        if (Vector512.IsHardwareAccelerated)
        {
            var shuffle = Vector512.Create(
                Vector256.Create(0, 2, 4, 6, 8, 10, 12, 14), 
                Vector256.Create(1, 3, 5, 7, 9, 11, 13, 15)
            );

            while (inCursor.Length >= 16)
            {
                var v = Vector512.Create(inCursor);
                v = Vector512.ShuffleNative(v, shuffle);
                v.GetLower().CopyTo(outCursor0);
                v.GetUpper().CopyTo(outCursor1);
                inCursor = inCursor[16..];
                outCursor0 = outCursor0[8..];
                outCursor1 = outCursor1[8..];
            }
        }
        else if (Vector256.IsHardwareAccelerated)
        {
            var shuffle = Vector256.Create(
                Vector128.Create(0, 2, 4, 6), 
                Vector128.Create(1, 3, 5, 7)
            );

            while (inCursor.Length >= 8)
            {
                var v = Vector256.Create(inCursor);
                v = Vector256.ShuffleNative(v, shuffle);
                v.GetLower().CopyTo(outCursor0);
                v.GetUpper().CopyTo(outCursor1);
                inCursor = inCursor[8..];
                outCursor0 = outCursor0[4..];
                outCursor1 = outCursor1[4..];
            }
        }
        else if (Vector128.IsHardwareAccelerated)
        {
            var shuffle = Vector128.Create(
                Vector64.Create(0, 2), 
                Vector64.Create(1, 3)
            );

            while (inCursor.Length >= 4)
            {
                var v = Vector128.Create(inCursor);
                v = Vector128.ShuffleNative(v, shuffle);
                v.GetLower().CopyTo(outCursor0);
                v.GetUpper().CopyTo(outCursor1);
                inCursor = inCursor[4..];
                outCursor0 = outCursor0[2..];
                outCursor1 = outCursor1[2..];
            } 
        }

        for (int i = 0, j = 0; i < inCursor.Length - 1; i += 2, j++)
        {
            outCursor0[j] = inCursor[i];
            outCursor1[j] = inCursor[i + 1];
        }
    }
}