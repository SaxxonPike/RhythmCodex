using RhythmCodex.Infrastructure;

namespace RhythmCodex.Extensions
{
    public static class ByteArrayExtensions
    {
        public static void Swap16(this byte[] array)
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
    }
}