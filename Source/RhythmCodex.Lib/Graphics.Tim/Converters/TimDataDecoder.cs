using System;
using RhythmCodex.IoC;

namespace RhythmCodex.Graphics.Tim.Converters;

[Service]
public class TimDataDecoder : ITimDataDecoder
{
    public int[] Decode4Bit(ReadOnlySpan<byte> data, int stride, int height)
    {
        var size = height * stride * 2;
        var result = new int[size];
        var sourceIdx = 0;

        for (var i = 0; i < size; i++)
        {
            var pixels = data[sourceIdx++];
            result[i++] = pixels & 0xF;
            result[i] = pixels >> 4;
        }

        return result;
    }

    public int[] Decode8Bit(ReadOnlySpan<byte> data, int stride, int height)
    {
        var size = height * stride;
        var result = new int[data.Length];

        for (var i = 0; i < size; i++)
            result[i] = data[i];

        return result;
    }

    public int[] Decode16Bit(ReadOnlySpan<byte> data, int stride, int height)
    {
        var size = height * stride / 2;
        var result = new int[size];
        var sourceIdx = 0;

        for (var i = 0; i < size; i++)
        {
            result[i] = ReadUInt16LittleEndian(data[sourceIdx..]);
            sourceIdx += 2;
        }

        return result;
    }

    public int[] Decode24Bit(ReadOnlySpan<byte> data, int stride, int height)
    {
        var size = height * stride / 3;
        var result = new int[size];
        var sourceIdx = 0;
        
        for (var i = 0; i < size; i++)
        {
            var pixels = data.Slice(sourceIdx, 3);
            result[i] = pixels[0] | (pixels[1] << 8) | (pixels[2] << 16);
            sourceIdx += 3;
        }

        return result;
    }
}