using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Sounds.Wav.Converters;

[Service]
public class PcmDecoder : IPcmDecoder
{
    public float[] Decode8Bit(ReadOnlySpan<byte> bytes)
    {
        var result = new float[bytes.Length];
        for (var i = 0; i < bytes.Length; i++)
            result[i] = bytes[i] / 128f;
        return result;
    }

    public float[] Decode16Bit(ReadOnlySpan<byte> bytes)
    {
        var result = new float[bytes.Length / 2];
        for (int i = 0, j = 0; i < bytes.Length - 1; i += 2)
            result[j++] = bytes[i..].AsS16L() / 32768f;
        return result;
    }

    public float[] Decode24Bit(ReadOnlySpan<byte> bytes)
    {
        var result = new float[bytes.Length / 3];
        for (int i = 0, j = 0; i < bytes.Length - 2; i += 3)
            result[j++] = bytes[i..].AsS24L() / 8388608f;
        return result;
    }

    public float[] Decode32Bit(ReadOnlySpan<byte> bytes)
    {
        var result = new float[bytes.Length / 4];
        for (int i = 0, j = 0; i < bytes.Length - 3; i += 4)
            result[j++] = bytes[i..].AsS32L() / 2147483648f;
        return result;
    }

    public float[] DecodeFloat(ReadOnlySpan<byte> bytes)
    {
        var result = new float[bytes.Length / 4];
        for (int i = 0, j = 0; i < bytes.Length - 3; i += 4)
            result[j++] = bytes[i..].AsF32L();
        return result;
    }
}