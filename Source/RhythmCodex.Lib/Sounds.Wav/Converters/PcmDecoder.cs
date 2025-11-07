using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Sounds.Wav.Converters;

[Service]
public class PcmDecoder : IPcmDecoder
{
    public float[] Decode8Bit(ReadOnlySpan<byte> bytes)
    {
        var result = new float[bytes.Length];
        for (var i = 0; i < bytes.Length; i++)
            result[i] = bytes[i] / 128f - 0.5f;
        return result;
    }

    public float[] Decode16Bit(ReadOnlySpan<byte> bytes)
    {
        var result = new float[bytes.Length / 2];
        for (int i = 0, j = 0; i < bytes.Length - 1; i += 2)
            result[j++] = Bitter.ToInt16(bytes[i..]) / 32768f;
        return result;
    }

    public float[] Decode24Bit(ReadOnlySpan<byte> bytes)
    {
        var result = new float[bytes.Length / 3];
        for (int i = 0, j = 0; i < bytes.Length - 2; i += 3)
            result[j++] = Bitter.ToInt24(bytes[i..]) / 8388608f;
        return result;
    }

    public float[] Decode32Bit(ReadOnlySpan<byte> bytes)
    {
        var result = new float[bytes.Length / 4];
        for (int i = 0, j = 0; i < bytes.Length - 3; i += 4)
            result[j++] = Bitter.ToInt32(bytes[i..]) / 2147483648f;
        return result;
    }

    public float[] DecodeFloat(ReadOnlySpan<byte> bytes)
    {
        var result = new float[bytes.Length / 4];
        for (int i = 0, j = 0; i < bytes.Length - 3; i += 4)
            result[j++] = Bitter.ToFloat(bytes[i..]);
        return result;
    }
}