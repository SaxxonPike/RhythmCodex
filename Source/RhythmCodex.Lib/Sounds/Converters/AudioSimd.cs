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
                ).AsInt16();

                for (var i = 0; i < 16; i++)
                {
                    target[startSample] = v[i];
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
                ).AsInt16();

                for (var i = 0; i < 8; i++)
                {
                    target[startSample] = v[i];
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
                ).AsInt16();

                for (var i = 0; i < 4; i++)
                {
                    target[startSample] = v[i];
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

    public static void Gain(Span<float> data, BigRational value)
    {
        if (value == BigRational.One)
            return;

        var amp = (float)value;
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
}