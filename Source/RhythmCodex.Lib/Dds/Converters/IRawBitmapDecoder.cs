using System;

namespace RhythmCodex.Dds.Converters;

public interface IRawBitmapDecoder
{
    int[] Decode32Bit(ReadOnlySpan<byte> src, int width, int height);
}