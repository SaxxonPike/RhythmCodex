using System;
using System.Diagnostics;
using System.Runtime.Intrinsics;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Extensions;

[DebuggerStepThrough]
internal static class ByteArrayExtensions
{
    extension(Span<byte> array)
    {
        public void Swap16()
        {
            if ((array.Length & 1) != 0)
                throw new RhythmCodexException("Array must have an even length in order to byte-swap 16-bit words.");

            if (Vector512.IsHardwareAccelerated && array.Length > 64)
            {
                var shuffler = Vector512.Create(
                    (byte)0x01, 0x00, 0x03, 0x02, 0x05, 0x04, 0x07, 0x06,
                    0x09, 0x08, 0x0B, 0x0A, 0x0D, 0x0C, 0x0F, 0x0E,
                    0x11, 0x10, 0x13, 0x12, 0x15, 0x14, 0x17, 0x16,
                    0x19, 0x18, 0x1B, 0x1A, 0x1D, 0x1C, 0x1F, 0x1E,
                    0x21, 0x20, 0x23, 0x22, 0x25, 0x24, 0x27, 0x26,
                    0x29, 0x28, 0x2B, 0x2A, 0x2D, 0x2C, 0x2F, 0x2E,
                    0x31, 0x30, 0x33, 0x32, 0x35, 0x34, 0x37, 0x36,
                    0x39, 0x38, 0x3B, 0x3A, 0x3D, 0x3C, 0x3F, 0x3E
                );

                while (array.Length >= 64)
                {
                    var vec = Vector512.ShuffleNative(Vector512.LoadUnsafe(ref array[0]), shuffler);
                    vec.StoreUnsafe(ref array[0]);
                    array = array[64..];
                }
            }
            else if (Vector256.IsHardwareAccelerated && array.Length > 32)
            {
                var shuffler = Vector256.Create(
                    (byte)0x01, 0x00, 0x03, 0x02, 0x05, 0x04, 0x07, 0x06,
                    0x09, 0x08, 0x0B, 0x0A, 0x0D, 0x0C, 0x0F, 0x0E,
                    0x11, 0x10, 0x13, 0x12, 0x15, 0x14, 0x17, 0x16,
                    0x19, 0x18, 0x1B, 0x1A, 0x1D, 0x1C, 0x1F, 0x1E
                );

                while (array.Length >= 32)
                {
                    var vec = Vector256.ShuffleNative(Vector256.LoadUnsafe(ref array[0]), shuffler);
                    vec.StoreUnsafe(ref array[0]);
                    array = array[32..];
                }
            }
            else if (Vector128.IsHardwareAccelerated && array.Length > 16)
            {
                var shuffler = Vector128.Create(
                    (byte)0x01, 0x00, 0x03, 0x02, 0x05, 0x04, 0x07, 0x06,
                    0x09, 0x08, 0x0B, 0x0A, 0x0D, 0x0C, 0x0F, 0x0E
                );

                while (array.Length >= 16)
                {
                    var vec = Vector128.ShuffleNative(Vector128.LoadUnsafe(ref array[0]), shuffler);
                    vec.StoreUnsafe(ref array[0]);
                    array = array[16..];
                }
            }

            array.Swap16NoSimd();
        }

        public void Swap16NoSimd()
        {
            for (var i = 0; i < array.Length; i += 2)
                WriteInt16LittleEndian(array[i..], ReadInt16BigEndian(array[i..]));
        }
    }
}