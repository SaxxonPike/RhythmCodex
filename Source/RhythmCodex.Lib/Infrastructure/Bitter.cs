using System;
using System.Linq;

namespace RhythmCodex.Infrastructure
{
    public static class Bitter
    {
        public static int[] ToInt16Array(ReadOnlySpan<byte> span, int offset, int count)
        {
            var output = new int[count];
            for (var i = 0; i < count; i++)
                output[i] = ToInt16(span, offset + i * 2);
            return output;
        }
        
        public static int ToInt16(ReadOnlySpan<byte> span, int offset) =>
            ToInt16(span[offset], span[offset + 1]);

        public static int ToInt16(byte lsb, byte msb) =>
            ((lsb << 16) | (msb << 24)) >> 16;

        public static int ToInt16S(ReadOnlySpan<byte> span) =>
            ToInt16S(span, 0);

        public static int ToInt16S(ReadOnlySpan<byte> span, int offset) =>
            ToInt16(span[offset + 1], span[offset]);

        public static int ToInt24(ReadOnlySpan<byte> span, int offset) =>
            ToInt24(span[offset], span[offset + 1], span[offset + 2]);

        public static int ToInt24(byte lsb, byte mid, byte msb) =>
            ((lsb << 8) | (mid << 16) | (msb << 24)) >> 8;

        public static int ToInt24S(ReadOnlySpan<byte> span) =>
            ToInt24S(span, 0);

        public static int ToInt24S(ReadOnlySpan<byte> span, int offset) =>
            ToInt24(span[offset + 2], span[offset + 1], span[offset]);

        public static int ToInt32(ReadOnlySpan<byte> span, int offset) =>
            ToInt32(span[offset], span[offset + 1], span[offset + 2], span[offset + 3]);

        public static int ToInt32(byte lsb, byte b, byte c, byte msb) =>
            lsb | (b << 8) | (c << 16) | (msb << 24);

        public static int ToInt32(ReadOnlySpan<byte> bytes) =>
            ToInt32(bytes[0], bytes[1], bytes[2], bytes[3]);

        public static int ToInt32S(ReadOnlySpan<byte> span) =>
            ToInt32S(span, 0);

        public static int ToInt32S(ReadOnlySpan<byte> span, int offset) =>
            ToInt32(span[offset + 3], span[offset + 2], span[offset + 1], span[offset]);
    }
}