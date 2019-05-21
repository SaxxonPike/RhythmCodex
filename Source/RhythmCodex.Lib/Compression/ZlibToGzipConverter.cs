using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Compression
{
    [Service]
    public class ZlibToGzipConverter : IZlibToGzipConverter
    {
        public byte[] Convert(byte[] zlibData)
        {
            var output = new byte[zlibData.Length + 10 - 2];
            output[0] = 0x1F;
            output[1] = 0x8B;
            output[2] = unchecked((byte)(zlibData[0] & 0xF));
            output[3] = 0x00;
            output[8] = GetXfl(zlibData[1]);
            output[9] = 0x00;
            zlibData.AsSpan().Slice(2).CopyTo(output.AsSpan().Slice(10));
            return output;
        }

        private byte GetXfl(byte b)
        {
            return 0x02;
        }
    }
}