using System;

namespace RhythmCodex.Graphics.Dds.Converters;

public interface IDxtDecoder
{
    int[] DecodeDxt1(ReadOnlySpan<byte> src, int width, int height, bool useAlpha);
    int[] DecodeDxt3(ReadOnlySpan<byte> src, int width, int height);
    int[] DecodeDxt5(ReadOnlySpan<byte> src, int width, int height);
}