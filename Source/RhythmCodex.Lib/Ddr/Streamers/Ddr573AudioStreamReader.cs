using System;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ddr.Streamers
{
    [Service]
    public class Ddr573AudioStreamReader : IDdr573AudioStreamReader
    {
        private const int KeySize = 0x1E00;

        public byte[] Read(
            Stream stream, 
            long length,
            ReadOnlySpan<byte> key,
            ReadOnlySpan<byte> scramble,
            int counter)
        {
            var data = new byte[length];
            stream.TryRead(data, 0, length);
            return Decrypt(data, key, scramble, counter);
        }

        private byte[] Decrypt(
            ReadOnlySpan<byte> data, 
            ReadOnlySpan<byte> key,
            ReadOnlySpan<byte> scramble,
            int counter)
        {
            var size = data.Length;
            var myCounter = counter;
            var result = new byte[size];
            for (var i = 0; i < size; i += 2)
            {
                var iv = (data[i] << 8) & 0x0000FFFF;
                iv |= data[i + 1] & 0xFF & 0x0000FFFF;
                var ov = 0;

                for (byte b = 0; b < 8; ++b)
                {
                    int b1 = BitIsSet(iv, (byte) (b * 2)), b2 = BitIsSet(iv, (byte) (b * 2 + 1));
                    if (BitIsSet(scramble[i / 2 % KeySize], b) == 1)
                    {
                        var t = b1;
                        b1 = b2;
                        b2 = t;
                    }

                    if ((b1 ^ BitIsSet(myCounter, (byte) (7 - b)) ^ BitIsSet(key[i / 2 % KeySize], b)) == 1)
                        ov |= 1 << (b * 2);
                    if ((b2 ^ BitIsSet(myCounter, b)) == 1)
                        ov |= 1 << (b * 2 + 1);
                }

                result[i + 1] = (byte) (ov & 0xFF);
                result[i] = (byte) (ov >> 8 & 0xFF);
                myCounter += 1;
                myCounter &= 0xFF;
            }

            return result;
        }

        private static int BitIsSet(byte v, byte i)
        {
            v &= 0xFF;
            i &= 0xFF;
            return (v & (1 << i)) != 0 ? 1 : 0;
        }

        private static int BitIsSet(int v, byte i)
        {
            return (v & (1 << i)) != 0 ? 1 : 0;
        }
    }
}