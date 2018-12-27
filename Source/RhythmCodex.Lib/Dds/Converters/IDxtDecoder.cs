using System;

namespace RhythmCodex.Dds.Converters
{
    public interface IDxtDecoder
    {
        int[] DecodeDxt1(ReadOnlySpan<byte> src, int width, int height);
    }
}