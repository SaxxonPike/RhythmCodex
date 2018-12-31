using System;
using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Infrastructure
{
    public static class Bitter
    {
        public static int ToInt16(ReadOnlySpan<byte> span, int offset) => 
            ToInt16(span[offset], span[offset + 1]);

        public static int ToInt16(byte lsb, byte msb) => 
            ((lsb << 16) | (msb << 24)) >> 16;

        public static int ToInt32(ReadOnlySpan<byte> span, int offset) => 
            ToInt32(span[offset], span[offset + 1], span[offset + 2], span[offset + 3]);

        public static int ToInt32(byte lsb, byte b, byte c, byte msb) => 
            lsb | (b << 8) | (c << 16) | (msb << 24);

        public static int ToInt32(IEnumerable<byte> bytes)
        {
            var data = bytes.Take(4).ToArray();
            var buffer = new byte[4];
            Array.Copy(data, buffer, data.Length);
            return ToInt32(buffer[0], buffer[1], buffer[2], buffer[3]);
        }

        public static int ToInt32(Span<byte> bytes) => 
            ToInt32(bytes[0], bytes[1], bytes[2], bytes[3]);
    }
}