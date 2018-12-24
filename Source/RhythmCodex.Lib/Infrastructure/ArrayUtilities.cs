using System;

namespace RhythmCodex.Infrastructure
{
    internal static class ArrayUtilities
    {
        /// <summary>
        /// Slice an array.
        /// </summary>
        public static T[] Slice<T>(this T[] arr, int offset) => 
            arr.AsSpan(offset).ToArray();

        /// <summary>
        /// Slice an array.
        /// </summary>
        public static T[] Slice<T>(this T[] arr, int offset, int length) => 
            arr.AsSpan(offset, length).ToArray();

        /// <summary>
        /// Combine 8-bit values into 16-bit values.
        /// </summary>
        public static short[] Fuse(this byte[] arr)
        {
            var output = new short[arr.Length / 2];
            Buffer.BlockCopy(arr, 0, output, 0, output.Length * 2);
            return output;
        }
        
        /// <summary>
        /// Combine 16-bit values into 32-bit values.
        /// </summary>
        public static int[] Fuse(this short[] arr)
        {
            var output = new int[arr.Length / 2];
            Buffer.BlockCopy(arr, 0, output, 0, output.Length * 4);
            return output;
        }

        /// <summary>
        /// Split 32-bit values into 16-bit values.
        /// </summary>
        public static short[] UnFuse(this int[] arr)
        {
            var output = new short[arr.Length * 2];
            Buffer.BlockCopy(arr, 0, output, 0, arr.Length * 4);
            return output;
        }
        
        /// <summary>
        /// Split 16-bit values into 8-bit values.
        /// </summary>
        public static byte[] UnFuse(this short[] arr)
        {
            var output = new byte[arr.Length * 2];
            Buffer.BlockCopy(arr, 0, output, 0, arr.Length * 2);
            return output;
        }
    }
}