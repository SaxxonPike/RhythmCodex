using System.Collections.Generic;

namespace RhythmCodex.Infrastructure.Converters
{
    public class Bcd : IBcd
    {
        public int FromBcd(IEnumerable<byte> bytes)
        {
            var output = 0;
            foreach (var b in bytes)
            {
                output *= 100;
                output += FromBcd(b);
            }

            return output;
        }

        public int FromBcd(byte b) => 
            (b & 0xF) + (b >> 4) * 10;
    }
}