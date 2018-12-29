using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Dds.Converters
{
    [Service]
    public class RawBitmapDecoder : IRawBitmapDecoder
    {
        public int[] Decode32Bit(ReadOnlySpan<byte> src, int width, int height)
        {
            var output = new int[width * height];
            var inPtr = 0;
            var outPtr = 0;

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                output[outPtr++] = src[inPtr++] |
                                   (src[inPtr++] << 8) |
                                   (src[inPtr++] << 16) |
                                   (src[inPtr++] << 24);
            }

            return output;
        }
    }
}