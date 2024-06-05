// using System;
// using System.Buffers.Binary;
// using JetBrains.Annotations;
//
// namespace RhythmCodex.Infrastructure;
//
// [PublicAPI]
// public static class ArrayUtilities
// {
//     /// <summary>
//     /// Combine 8-bit values into 16-bit values. (Little endian.)
//     /// </summary>
//     public static short[] FuseLE(this ReadOnlySpan<byte> arr)
//     {
//         var output = new short[arr.Length / 2];
//         for (var i = 0; i < output.Length; i++)
//             output[i] = BinaryPrimitives.ReadInt16LittleEndian(arr[(i << 1)..]);
//         return output;
//     }
//
//     /// <summary>
//     /// Combine 8-bit values into 16-bit values. (Big endian.)
//     /// </summary>
//     public static short[] FuseBE(this ReadOnlySpan<byte> arr)
//     {
//         var output = new short[arr.Length / 2];
//         for (var i = 0; i < output.Length; i++)
//             output[i] = BinaryPrimitives.ReadInt16BigEndian(arr[(i << 1)..]);
//         return output;
//     }
//
//     /// <summary>
//     /// Split 16-bit values into 8-bit values.
//     /// </summary>
//     public static byte[] UnFuse(this short[] arr)
//     {
//         var output = new byte[arr.Length * 2];
//         Buffer.BlockCopy(arr, 0, output, 0, arr.Length * 2);
//         return output;
//     }
// }