using System;

namespace RhythmCodex.Graphics.Dds.Converters;

public interface IRawBitmapDecoder
{
    int[] Decode32Bit(ReadOnlySpan<byte> src, int width, int height);
}