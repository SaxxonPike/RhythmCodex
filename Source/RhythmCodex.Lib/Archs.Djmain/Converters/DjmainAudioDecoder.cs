using System;
using System.Buffers.Binary;
using System.Runtime.Intrinsics;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Djmain.Converters;

/// <inheritdoc />
[Service]
public class DjmainAudioDecoder : IDjmainAudioDecoder
{
    /// <summary>
    /// Delta values for 4-bit DPCM mode.
    /// </summary>
    private static readonly int[] DpcmTable =
    [
        0x00, 0x01, 0x02, 0x04,
        0x08, 0x10, 0x20, 0x40,
        0x00, 0xC0, 0xE0, 0xF0,
        0xF8, 0xFC, 0xFE, 0xFF
    ];

    /// <inheritdoc />
    public float[] DecodeDpcm(ReadOnlySpan<byte> data)
    {
        //
        // Each byte contains two 4-bit values which correspond to the DPCM table.
        // These are delta values applied to an 8-bit accumulator. The lower 4 bits
        // are processed before the upper 4 bits.
        //

        var result = new float[data.Length * 2];
        var inIndex = 0;
        var outIndex = 0;
        var accumulator = 0x00;

        while (outIndex < result.Length)
        {
            var inByte = data[inIndex++];

            accumulator = (accumulator + DpcmTable[inByte & 0xF]) & 0xFF;
            result[outIndex++] = ((accumulator ^ 0x80) - 0x80) / 128f;

            accumulator = (accumulator + DpcmTable[inByte >> 4]) & 0xFF;
            result[outIndex++] = ((accumulator ^ 0x80) - 0x80) / 128f;
        }

        return result;
    }

    /// <inheritdoc />
    public float[] DecodePcm8(ReadOnlySpan<byte> data)
    {
        //
        // 8-bit data is signed.
        //

        var result = new float[data.Length];
        var outCursor = result.AsSpan();
        var index = 0;

        while (index < result.Length)
        {
            outCursor[index] = ((data[index] ^ 0x80) - 0x80) / 128f;
            index++;
        }

        return result;
    }

    /// <inheritdoc />
    public float[] DecodePcm16(ReadOnlySpan<byte> data)
    {
        //
        // 16-bit data is signed, little endian.
        //

        var result = new float[(data.Length + 1) / 2];
        var inIndex = 0;
        var inLimit = data.Length & ~1;
        var outIndex = 0;

        while (inIndex < inLimit)
        {
            result[outIndex++] = BinaryPrimitives.ReadInt16LittleEndian(data[inIndex..]) / 32768f;
            inIndex += 2;
        }

        //
        // In the event of a partial final sample, the last byte is treated as the lower 8 bits
        // and the upper 8 bits are set to zero.
        // 
        
        if (outIndex < result.Length)
        {
            result[outIndex] = data[inIndex] / 32768f;
        }

        return result;
    }
}