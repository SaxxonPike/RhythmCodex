using System.Collections.Generic;

namespace RhythmCodex.Infrastructure
{
    public static class Bcd
    {
        public static int Decode(IEnumerable<byte> bytes)
        {
            var output = 0;
            foreach (var b in bytes)
            {
                output *= 100;
                output += Decode(b);
            }

            return output;
        }

        public static int Decode(byte b) => 
            (b & 0xF) + (b >> 4) * 10;

        public static byte Encode(int data)
        {
            return unchecked((byte) (((data / 10) << 4) | (data % 10)));
        }
    }
}