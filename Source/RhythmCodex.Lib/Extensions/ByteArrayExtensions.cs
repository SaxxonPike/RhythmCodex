using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Extensions
{
    public static class ByteArrayExtensions
    {
        public static void Swap16(this Span<byte> array)
        {
            if ((array.Length & 1) != 0)
                throw new RhythmCodexException("Array must have an even length in order to byte-swap 16-bit words.");

            for (var i = 0; i < array.Length; i += 2)
            {
                var temp = array[i];
                array[i] = array[i + 1];
                array[i + 1] = temp;
            }
        }

        public static byte[] Pad(this byte[] array, int length) => Pad(array.AsSpan(), length);
        
        public static byte[] Pad(this Span<byte> array, int length) => Pad((ReadOnlySpan<byte>) array, length);

        public static byte[] Pad(this ReadOnlySpan<byte> array, int length)
        {
            var bytesToAdd = length - array.Length;
            if (bytesToAdd <= 0)
                return array.ToArray();
            var result = new byte[array.Length + bytesToAdd];
            array.CopyTo(result);
            return result;
        }
    }
}