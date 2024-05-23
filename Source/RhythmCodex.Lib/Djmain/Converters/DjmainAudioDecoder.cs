using System;
using RhythmCodex.IoC;

namespace RhythmCodex.Djmain.Converters;

[Service]
public class DjmainAudioDecoder : IDjmainAudioDecoder
{
    private static readonly int[] DpcmTable =
    {
        0x00, 0x01, 0x02, 0x04,
        0x08, 0x10, 0x20, 0x40,
        0x00, 0xC0, 0xE0, 0xF0,
        0xF8, 0xFC, 0xFE, 0xFF
    };

    public float[] DecodeDpcm(ReadOnlySpan<byte> data)
    {
        var result = new float[data.Length * 2];
        var inCursor = data;
        var outCursor = result.AsSpan();
        
        var accumulator = 0x00;

        while (inCursor.Length >= 1)
        {
            accumulator = (accumulator + DpcmTable[inCursor[0] & 0xF]) & 0xFF;
            outCursor[0] = ((accumulator ^ 0x80) - 0x80) / 128f;
            accumulator = (accumulator + DpcmTable[inCursor[0] >> 4]) & 0xFF;
            outCursor[1] = ((accumulator ^ 0x80) - 0x80) / 128f;
            inCursor = inCursor[1..];
            outCursor = outCursor[2..];
        }

        return result;
    }

    public float[] DecodePcm8(ReadOnlySpan<byte> data)
    {
        var result = new float[data.Length];
        var inCursor = data;
        var outCursor = result.AsSpan();

        while (inCursor.Length >= 1)
        {
            outCursor[0] = ((inCursor[0] ^ 0x80) - 0x80) / 128f;
            inCursor = inCursor[1..];
            outCursor = outCursor[1..];
        }

        return result;
    }

    public float[] DecodePcm16(ReadOnlySpan<byte> data)
    {
        var result = new float[(data.Length + 1) / 2];
        var inCursor = data;
        var outCursor = result.AsSpan();

        while (inCursor.Length > 0)
        {
            var low = inCursor[0];
            var high = inCursor.Length > 1 ? inCursor[1] : 0;
            outCursor[0] = (((low | (high << 8)) << 16) >> 16) / 32768f;

            if (inCursor.Length < 2)
                break;

            inCursor = inCursor[2..];
            outCursor = outCursor[1..];
        }

        return result;
    }
}