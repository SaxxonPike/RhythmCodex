using System;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Converters
{
    [Service]
    public class Ddr573DatabaseDecrypter : IDdr573DatabaseDecrypter
    {
        public byte[] Decrypt(ReadOnlySpan<byte> database, string key)
        {
            var val = 0x41C64E6D;
            var key1 = unchecked(val * CalculateKey(key));
            var counter = 0;
            var output = new byte[database.Length];

            for (var idx = 0; idx < database.Length; idx++)
            {
                val = ((key1 + counter) >> 5) ^ database[idx];
                output[idx] = unchecked((byte) val);
                counter += 0x3039;
            }

            return output;
        }

        private static int CalculateKey(string input)
        {
            var key = 0;

            foreach (var c in input)
            {
                if (char.IsUpper(c))
                    key -= 0x37;
                else if (char.IsLower(c))
                    key -= 0x57;
                else if (char.IsDigit(c))
                    key -= 0x30;
                key += c;
            }

            return unchecked((byte) key);
        }
    }
}