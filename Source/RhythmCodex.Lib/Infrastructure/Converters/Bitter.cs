using System;
using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Infrastructure.Converters
{
    public class Bitter : IBitter
    {
        public int ToInt32(byte lsb, byte b, byte c, byte msb)
        {
            return lsb | b << 8 | c << 16 | msb << 24;
        }

        public int ToInt32(IEnumerable<byte> bytes)
        {
            var data = bytes.Take(4).ToArray();
            var buffer = new byte[4];
            Array.Copy(data, buffer, data.Length);
            return ToInt32(buffer[0], buffer[1], buffer[2], buffer[3]);
        }
    }
}