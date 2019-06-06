using System;
using System.IO;
using RhythmCodex.Compression;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Converters
{
    [Service]
    public class Ddr573DatabaseDecrypter : IDdr573DatabaseDecrypter
    {
        private readonly IBemaniLzDecoder _bemaniLzDecoder;

        public Ddr573DatabaseDecrypter(IBemaniLzDecoder bemaniLzDecoder)
        {
            _bemaniLzDecoder = bemaniLzDecoder;
        }

        private static bool IsLetter(byte value)
        {
            return (value >= 0x41 && value <= 0x5A) ||
                   (value >= 0x61 && value <= 0x7A);
        }

        private static bool IsLetterOrDigit(byte value)
        {
            return (value >= 0x41 && value <= 0x5A) ||
                   (value >= 0x61 && value <= 0x7A) ||
                   (value >= 0x30 && value <= 0x39);
        }
        
        public int FindKey(ReadOnlySpan<byte> database)
        {
            var header = database.Slice(0, 16);
            for (var i = 0; i < 256; i++)
            {
                var test = _bemaniLzDecoder.Decode(new MemoryStream(Decrypt(header, i)));
                
                if (!IsLetter(test[0]))
                    continue;
                if (!IsLetter(test[1]))
                    continue;
                if (!IsLetter(test[2]))
                    continue;
                if (!IsLetterOrDigit(test[3]))
                    continue;
                if (!IsLetterOrDigit(test[4]) && test[4] != 0)
                    continue;
                if (test[5] != 0)
                    continue;
                return i;
            }

            throw new RhythmCodexException("Can't seem to find the key for this MDB");
        }

        public byte[] Decrypt(ReadOnlySpan<byte> database, int key)
        {
            var val = 0x41C64E6D;
            var key1 = unchecked(val * key);
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

        public int ConvertKey(string input)
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