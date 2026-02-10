using System;
using System.Numerics;
using System.Runtime.Intrinsics;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Converters;

/// <summary>
/// Contains vectorized functions for audio processing.
/// </summary>
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
        Span<short> target,
        ReadOnlySpan<float> source,
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

        if (Vector256.IsHardwareAccelerated)
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

        if (Vector128.IsHardwareAccelerated)
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

    /// <summary>
    /// Sums source audio data into a target.
    /// </summary>
    public static void Mix(Span<float> target, ReadOnlySpan<float> source)
    {
        var targetCursor = target;
        var sourceCursor = source;
        var maxLength = Math.Min(target.Length, source.Length);

        if (Vector512.IsHardwareAccelerated)
        {
            while (maxLength >= 16)
            {
                var v = Vector512.Create(targetCursor);
                v += Vector512.Create(sourceCursor);
                v.CopyTo(targetCursor);
                targetCursor = targetCursor[16..];
                sourceCursor = sourceCursor[16..];
                maxLength -= 16;
            }
        }

        if (Vector256.IsHardwareAccelerated)
        {
            while (maxLength >= 8)
            {
                var v = Vector256.Create(targetCursor);
                v += Vector256.Create(sourceCursor);
                v.CopyTo(targetCursor);
                targetCursor = targetCursor[8..];
                sourceCursor = sourceCursor[8..];
                maxLength -= 8;
            }
        }

        if (Vector128.IsHardwareAccelerated)
        {
            while (maxLength >= 4)
            {
                var v = Vector128.Create(targetCursor);
                v += Vector128.Create(sourceCursor);
                v.CopyTo(targetCursor);
                targetCursor = targetCursor[4..];
                sourceCursor = sourceCursor[4..];
                maxLength -= 4;
            }
        }

        if (Vector64.IsHardwareAccelerated)
        {
            while (maxLength >= 2)
            {
                var v = Vector64.Create(targetCursor);
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

    /// <summary>
    /// Applies gain to source and target, then sums the result into the target.
    /// </summary>
    public static void MixGain(
        Span<float> target,
        float targetAmp,
        ReadOnlySpan<float> source,
        float sourceAmp
    )
    {
        var targetCursor = target;
        var sourceCursor = source;
        var maxLength = Math.Min(target.Length, source.Length);

        if (Vector512.IsHardwareAccelerated)
        {
            while (maxLength >= 16)
            {
                var v = Vector512.Create(targetCursor) * targetAmp;
                v += Vector512.Create(sourceCursor) * sourceAmp;
                v.CopyTo(targetCursor);
                targetCursor = targetCursor[16..];
                sourceCursor = sourceCursor[16..];
                maxLength -= 16;
            }
        }

        if (Vector256.IsHardwareAccelerated)
        {
            while (maxLength >= 8)
            {
                var v = Vector256.Create(targetCursor) * targetAmp;
                v += Vector256.Create(sourceCursor) * sourceAmp;
                v.CopyTo(targetCursor);
                targetCursor = targetCursor[8..];
                sourceCursor = sourceCursor[8..];
                maxLength -= 8;
            }
        }

        if (Vector128.IsHardwareAccelerated)
        {
            while (maxLength >= 4)
            {
                var v = Vector128.Create(targetCursor) * targetAmp;
                v += Vector128.Create(sourceCursor) * sourceAmp;
                v.CopyTo(targetCursor);
                targetCursor = targetCursor[4..];
                sourceCursor = sourceCursor[4..];
                maxLength -= 4;
            }
        }

        if (Vector64.IsHardwareAccelerated)
        {
            while (maxLength >= 2)
            {
                var v = Vector64.Create(targetCursor) * targetAmp;
                v += Vector64.Create(sourceCursor) * sourceAmp;
                v.CopyTo(targetCursor);
                targetCursor = targetCursor[2..];
                sourceCursor = sourceCursor[2..];
                maxLength -= 2;
            }
        }

        for (var i = 0; i < maxLength; i++)
            targetCursor[i] = targetCursor[i] * targetAmp + sourceCursor[i] * sourceAmp;
    }

    /// <summary>
    /// Applies gain to target audio data.
    /// </summary>
    public static void Gain(Span<float> target, float amp)
    {
        if (amp == 1)
            return;

        var cursor = target;

        if (Vector512.IsHardwareAccelerated)
        {
            while (cursor.Length >= 16)
            {
                var v = Vector512.Create(cursor) * amp;
                v.CopyTo(cursor);
                cursor = cursor[16..];
            }
        }

        if (Vector256.IsHardwareAccelerated)
        {
            while (cursor.Length >= 8)
            {
                var v = Vector256.Create(cursor) * amp;
                v.CopyTo(cursor);
                cursor = cursor[8..];
            }
        }

        if (Vector128.IsHardwareAccelerated)
        {
            while (cursor.Length >= 4)
            {
                var v = Vector128.Create(cursor) * amp;
                v.CopyTo(cursor);
                cursor = cursor[4..];
            }
        }

        if (Vector64.IsHardwareAccelerated)
        {
            while (cursor.Length >= 2)
            {
                var v = Vector64.Create(cursor) * amp;
                v.CopyTo(cursor);
                cursor = cursor[2..];
            }
        }

        for (var i = 0; i < cursor.Length; i++)
            cursor[i] *= amp;
    }

    /// <summary>
    /// Converts signed 8-bit audio to floats.
    /// </summary>
    public static void Raw8ToFloats(
        ReadOnlySpan<byte> source,
        Span<float> target,
        AudioSign audioSign
    )
    {
        var inMax = Math.Min(source.Length, target.Length);
        var inCursor = source[..inMax];
        var outCursor = target[..inMax];
        var sign = audioSign switch
        {
            AudioSign.Unsigned => 1 << 31,
            _ => 0
        };
        const float midpoint = 0x80;

        if (Vector<int>.IsSupported && Vector<float>.IsSupported && Vector<int>.Count == Vector<float>.Count)
        {
            var blockSize = Vector<int>.Count;
            Span<int> inBuffer = stackalloc int[blockSize];
            var vecSign = Vector.Create(sign);
            var vecMidpoint = Vector.Create(midpoint);

            while (inCursor.Length >= blockSize)
            {
                for (var i = 0; i < blockSize; i++)
                    inBuffer[i] = inCursor[i];
                inCursor = inCursor[blockSize..];

                var v = Vector.ConvertToSingle(
                    ((Vector.Create(inBuffer) << 24) ^ vecSign) >> 24
                ) / vecMidpoint;

                v.StoreUnsafe(ref outCursor[0]);
                outCursor = outCursor[blockSize..];
            }
        }

        //
        // Fallback.
        //

        for (var i = 0; i < inCursor.Length; i++)
            outCursor[i] = (((inCursor[i] ^ sign) << 24) >> 24) / 128f;
    }

    /// <summary>
    /// Converts signed 16-bit audio to floats.
    /// </summary>
    public static void Raw16ToFloats(
        ReadOnlySpan<byte> source,
        Span<float> target,
        AudioSign audioSign,
        AudioEndian endian
    )
    {
        var inMax = Math.Min(source.Length / 2, target.Length);
        var inCursor = source[..(inMax * 2)];
        var outCursor = target[..inMax];
        var sign = audioSign switch
        {
            AudioSign.Unsigned => 1 << 31,
            _ => 0
        };
        const float midpoint = 0x8000;

        if (Vector<int>.IsSupported && Vector<float>.IsSupported && Vector<int>.Count == Vector<float>.Count)
        {
            var blockSize = Vector<int>.Count;
            var inByteSize = blockSize * 2;

            Span<int> inBuffer = stackalloc int[blockSize];
            var vecSign = Vector.Create(sign);
            var vecMidpoint = Vector.Create(midpoint);

            while (inCursor.Length >= inByteSize)
            {
                switch (endian)
                {
                    case AudioEndian.Big:
                        for (int i = 0, j = 0; i < blockSize; i++, j += 2)
                            inBuffer[i] = ReadInt16BigEndian(inCursor[j..]);
                        break;
                    case AudioEndian.Little:
                    default:
                        for (int i = 0, j = 0; i < blockSize; i++, j += 2)
                            inBuffer[i] = ReadInt16LittleEndian(inCursor[j..]);
                        break;
                }

                inCursor = inCursor[inByteSize..];

                var v0 = ((Vector.Create(inBuffer) << 16) ^ vecSign) >> 16;
                var v1 = Vector.ConvertToSingle(v0) / vecMidpoint;

                v1.StoreUnsafe(ref outCursor[0]);
                outCursor = outCursor[blockSize..];
            }
        }

        //
        // Fallback.
        //

        switch (endian)
        {
            case AudioEndian.Big:
                for (int i = 0, j = 0; j < inCursor.Length; i++, j += 2)
                    outCursor[i] = (((ReadInt16BigEndian(inCursor[j..]) << 16) ^ sign) >> 16) / midpoint;
                break;
            case AudioEndian.Little:
            default:
                for (int i = 0, j = 0; j < inCursor.Length; i++, j += 2)
                    outCursor[i] = (((ReadInt16LittleEndian(inCursor[j..]) << 16) ^ sign) >> 16) / midpoint;
                break;
        }
    }

    /// <summary>
    /// Deinterleaves stereo audio data into two targets.
    /// </summary>
    public static void Deinterleave2(ReadOnlySpan<float> source, Span<float> target0, Span<float> target1)
    {
        var inCursor = source;
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

        if (Vector256.IsHardwareAccelerated)
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

        if (Vector128.IsHardwareAccelerated)
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