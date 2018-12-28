using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Dds.Converters
{
    [Service]
    public class DxtDecoder : IDxtDecoder
    {
        public int[] DecodeDxt1(ReadOnlySpan<byte> src, int width, int height, bool useAlpha)
        {
            // reference: https://github.com/toji/webgl-texture-utils/blob/master/texture-util/dds.js

            unchecked
            {
                var c = new int[4];
                var dst = new int[width * height];
                var blockWidth = width / 4;
                var blockHeight = height / 4;

                for (var blockY = 0; blockY < blockHeight; blockY++)
                for (var blockX = 0; blockX < blockWidth; blockX++)
                {
                    var srcBlock = src.Slice(8 * (blockY * blockWidth + blockX));
                    var dstBlock = dst.AsSpan(blockY * 4 * width + blockX * 4);
                    DecodeColorBlock(srcBlock, c);
                    if (useAlpha && c[0] <= c[1])
                        ConvertColorBlockWithAlpha(srcBlock, dstBlock, c, width);
                    else
                        ConvertColorBlock(srcBlock, dstBlock, c, width);
                }

                return dst;
            }
        }

        public int[] DecodeDxt3(ReadOnlySpan<byte> src, int width, int height)
        {
            unchecked
            {
                var c = new int[4];
                var dst = new int[width * height];
                var blockWidth = width / 4;
                var blockHeight = height / 4;

                for (var blockY = 0; blockY < blockHeight; blockY++)
                for (var blockX = 0; blockX < blockWidth; blockX++)
                {
                    var srcBlock = src.Slice(16 * (blockY * blockWidth + blockX));
                    var dstBlock = dst.AsSpan(blockY * 4 * width + blockX * 4);
                    DecodeColorBlock(srcBlock.Slice(8), c);
                    ConvertColorBlockWithDiscreteAlpha(srcBlock, dstBlock, c, width);
                }

                return dst;
            }
        }
        
        public int[] DecodeDxt5(ReadOnlySpan<byte> src, int width, int height)
        {
            unchecked
            {
                var c = new int[4];
                var dst = new int[width * height];
                var blockWidth = width / 4;
                var blockHeight = height / 4;

                for (var blockY = 0; blockY < blockHeight; blockY++)
                for (var blockX = 0; blockX < blockWidth; blockX++)
                {
                    var srcBlock = src.Slice(16 * (blockY * blockWidth + blockX));
                    var dstBlock = dst.AsSpan(blockY * 4 * width + blockX * 4);
                    DecodeColorBlock(srcBlock.Slice(8), c);
                    ConvertColorBlockWithCompositeAlpha(srcBlock, dstBlock, c, width);
                }

                return dst;
            }
        }
        
        private static void ConvertColorBlock(ReadOnlySpan<byte> src, Span<int> dst, ReadOnlySpan<int> c, int width)
        {
            var dstI = 0;
            for (var j = 4; j < 8; j++)
            {
                var m = src[j];
                dst[dstI] = Convert16To32(c, m & 0x3);
                dst[dstI + 1] = Convert16To32(c, (m >> 2) & 0x3);
                dst[dstI + 2] = Convert16To32(c, (m >> 4) & 0x3);
                dst[dstI + 3] = Convert16To32(c, m >> 6);
                dstI += width;
            }
        }

        private static void ConvertColorBlockWithAlpha(ReadOnlySpan<byte> src, Span<int> dst, ReadOnlySpan<int> c,
            int width)
        {
            var dstI = 0;
            for (var j = 4; j < 8; j++)
            {
                var m = src[j];
                dst[dstI] = (m & 0x3) == 0x3 ? 0x00000000 : Convert16To32(c, m & 0x3);
                dst[dstI + 1] = (m & 0xC) == 0xC ? 0x00000000 : Convert16To32(c, (m >> 2) & 0x3);
                dst[dstI + 2] = (m & 0x30) == 0x30 ? 0x00000000 : Convert16To32(c, (m >> 4) & 0x3);
                dst[dstI + 3] = (m & 0xC0) == 0xC0 ? 0x00000000 : Convert16To32(c, m >> 6);
                dstI += width;
            }
        }

        private static void ConvertColorBlockWithDiscreteAlpha(ReadOnlySpan<byte> src, Span<int> dst, ReadOnlySpan<int> c,
            int width)
        {
            var dstI = 0;
            for (int j = 12, k = 0; j < 16; j++)
            {
                var m = src[j];
                dst[dstI] = Convert16ATo32(c, m & 0x3, (src[k] & 0xF) * 0x11);
                dst[dstI + 1] = Convert16ATo32(c, (m >> 2) & 0x3, (src[k++] >> 4) * 0x11);
                dst[dstI + 2] = Convert16ATo32(c, (m >> 4) & 0x3, (src[k] & 0xF) * 0x11);
                dst[dstI + 3] = Convert16ATo32(c, m >> 6, (src[k++] >> 4) * 0x11);
                dstI += width;
            }
        }

        private static void ConvertColorBlockWithCompositeAlpha(ReadOnlySpan<byte> src, Span<int> dst, ReadOnlySpan<int> c,
            int width)
        {
            // refer here: https://www.khronos.org/registry/OpenGL/extensions/EXT/EXT_texture_compression_s3tc.txt
            var a0 = src[0];
            var a1 = src[1];
            var a = a0 > a1
                ? new[]
                {
                    a0,
                    a1,
                    (6 * a0 + 1 * a1) / 7,
                    (5 * a0 + 2 * a1) / 7,
                    (4 * a0 + 3 * a1) / 7,
                    (3 * a0 + 4 * a1) / 7,
                    (2 * a0 + 5 * a1) / 7,
                    (1 * a0 + 6 * a1) / 7
                }
                : new[]
                {
                    a0,
                    a1,
                    (4 * a0 + 1 * a1) / 5,
                    (3 * a0 + 2 * a1) / 5,
                    (2 * a0 + 3 * a1) / 5,
                    (1 * a0 + 4 * a1) / 5,
                    0,
                    255
                };
            var n = src[2] |
                    ((long) src[3] << 8) |
                    ((long) src[4] << 16) |
                    ((long) src[5] << 24) |
                    ((long) src[6] << 32) |
                    ((long) src[7] << 40);
            var dstI = 0;
            for (var j = 12; j < 16; j++)
            {
                var m = src[j];
                dst[dstI] = Convert16ATo32(c, m & 0x3, a[n & 0x7]);
                dst[dstI + 1] = Convert16ATo32(c, (m >> 2) & 0x3, a[(n >> 3) & 0x7]);
                dst[dstI + 2] = Convert16ATo32(c, (m >> 4) & 0x3, a[(n >> 6) & 0x7]);
                dst[dstI + 3] = Convert16ATo32(c, m >> 6, a[(n >> 9) & 0x7]);
                dstI += width;
                n >>= 12;
            }
            
            throw new NotImplementedException();
        }
        
        private static void DecodeColorBlock(ReadOnlySpan<byte> src, Span<int> c)
        {
            c[0] = src[0] | (src[1] << 8);
            c[1] = src[2] | (src[3] << 8);
            var r0 = c[0] & 0x1f;   // ........ ...xxxxx ........ ........
            var g0 = c[0] & 0x7e0;  // .....xxx xxx..... ........ ........
            var b0 = c[0] & 0xf800; // xxxxx... ........ ........ ........
            var r1 = c[1] & 0x1f;   // ........ ........ ........ ...xxxxx
            var g1 = c[1] & 0x7e0;  // ........ ........ .....xxx xxx.....
            var b1 = c[1] & 0xf800; // ........ ........ xxxxx... ........
            c[2] = ((5 * r0 + 3 * r1) >> 3)
                   | (((5 * g0 + 3 * g1) >> 3) & 0x7e0)
                   | (((5 * b0 + 3 * b1) >> 3) & 0xf800);
            c[3] = ((5 * r1 + 3 * r0) >> 3)
                   | (((5 * g1 + 3 * g0) >> 3) & 0x7e0)
                   | (((5 * b1 + 3 * b0) >> 3) & 0xf800);
        }

        private static int Convert16To32(ReadOnlySpan<int> colors, int code)
        {
            var color = colors[code];
            return ((color << 3) & 0x000000FF) | // ........ ...xxxxx -> ........ ........ xxxxx000
                   ((color << 5) & 0x0000FF00) | // .....xxx xxx..... -> ........ xxxxxx00 ........
                   ((color << 8) & 0x00FF0000) | // xxxxx... ........ -> xxxxx000 ........ ........
                   ~0x00FFFFFF; // full alpha
        }
        
        private static int Convert16ATo32(ReadOnlySpan<int> colors, int code, int alpha)
        {
            var color = colors[code];
            return ((color << 3) & 0x000000FF) | // ........ ...xxxxx -> ........ ........ xxxxx000
                   ((color << 5) & 0x0000FF00) | // .....xxx xxx..... -> ........ xxxxxx00 ........
                   ((color << 8) & 0x00FF0000) | // xxxxx... ........ -> xxxxx000 ........ ........
                   (alpha << 24); // full alpha
        }
    }
}