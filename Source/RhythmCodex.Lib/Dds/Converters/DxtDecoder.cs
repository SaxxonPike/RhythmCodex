using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Dds.Converters
{
    [Service]
    public class DxtDecoder : IDxtDecoder
    {
        private int Convert16to32(int color)
        {
            return ((color << 3) & 0x000000FF) | // ........ ...xxxxx -> ........ ........ xxxxx000
                   ((color << 5) & 0x0000FF00) | // .....xxx xxx..... -> ........ xxxxxx00 ........
                   ((color << 8) & 0x00FF0000) | // xxxxx... ........ -> xxxxx000 ........ ........
                   ~0x00FFFFFF; // full alpha
        }

        public int[] DecodeDxt1(ReadOnlySpan<byte> src, int width, int height)
        {
            // reference: https://github.com/toji/webgl-texture-utils/blob/master/texture-util/dds.js

            unchecked
            {
                var c = new int[4];
                var dst = new int[width * height];
                var blockWidth = width / 4;
                var blockHeight = height / 4;

                for (var blockY = 0; blockY < blockHeight; blockY++)
                {
                    for (var blockX = 0; blockX < blockWidth; blockX++)
                    {
                        var i = 8 * (blockY * blockWidth + blockX);
                        c[0] = src[i] | (src[i + 1] << 8);
                        c[1] = src[i + 2] | (src[i + 3] << 8);
                        var r0 = c[0] & 0x1f;
                        var g0 = c[0] & 0x7e0;
                        var b0 = c[0] & 0xf800;
                        var r1 = c[1] & 0x1f;
                        var g1 = c[1] & 0x7e0;
                        var b1 = c[1] & 0xf800;
                        c[2] = ((5 * r0 + 3 * r1) >> 3)
                               | (((5 * g0 + 3 * g1) >> 3) & 0x7e0)
                               | (((5 * b0 + 3 * b1) >> 3) & 0xf800);
                        c[3] = ((5 * r1 + 3 * r0) >> 3)
                               | (((5 * g1 + 3 * g0) >> 3) & 0x7e0)
                               | (((5 * b1 + 3 * b0) >> 3) & 0xf800);
                        for (int dstI = blockY * 4 * width + blockX * 4, j = 4; j < 8; j++)
                        {
                            var m = src[i + j];
                            dst[dstI] = Convert16to32(c[m & 0x3]);
                            dst[dstI + 1] = Convert16to32(c[(m >> 2) & 0x3]);
                            dst[dstI + 2] = Convert16to32(c[(m >> 4) & 0x3]);
                            dst[dstI + 3] = Convert16to32(c[(m >> 6) & 0x3]);
                            dstI += width;
                        }
                    }
                }

                return dst;
            }
        }
    }
}