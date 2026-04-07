using System;
using System.Diagnostics;
using System.Runtime.Intrinsics;

namespace RhythmCodex.Extensions;

[DebuggerStepThrough]
public static class ByteArrayExtensions
{
    extension(Span<byte> array)
    {
        public void Swap16()
        {
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
            for (var i = 0; i < array.Length - 1; i += 2)
                WriteInt16LittleEndian(array[i..], ReadInt16BigEndian(array[i..]));
        }
        
        public void Swap32()
        {
            if (Vector512.IsHardwareAccelerated && array.Length > 64)
            {
                var shuffler = Vector512.Create(
                    (byte)0x03, 0x02, 0x01, 0x00, 0x07, 0x06, 0x05, 0x04,
                    0x0B, 0x0A, 0x09, 0x08, 0x0F, 0x0E, 0x0D, 0x0C,
                    0x13, 0x12, 0x11, 0x10, 0x17, 0x16, 0x15, 0x14,
                    0x1B, 0x1A, 0x19, 0x18, 0x1F, 0x1E, 0x1D, 0x1C,
                    0x23, 0x22, 0x21, 0x20, 0x27, 0x26, 0x25, 0x24,
                    0x2B, 0x2A, 0x29, 0x28, 0x2F, 0x2E, 0x2D, 0x2C,
                    0x33, 0x32, 0x31, 0x30, 0x37, 0x36, 0x35, 0x34,
                    0x3B, 0x3A, 0x39, 0x38, 0x3F, 0x3E, 0x3D, 0x3C
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
                    (byte)0x03, 0x02, 0x01, 0x00, 0x07, 0x06, 0x05, 0x04,
                    0x0B, 0x0A, 0x09, 0x08, 0x0F, 0x0E, 0x0D, 0x0C,
                    0x13, 0x12, 0x11, 0x10, 0x17, 0x16, 0x15, 0x14,
                    0x1B, 0x1A, 0x19, 0x18, 0x1F, 0x1E, 0x1D, 0x1C
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
                    (byte)0x03, 0x02, 0x01, 0x00, 0x07, 0x06, 0x05, 0x04,
                    0x0B, 0x0A, 0x09, 0x08, 0x0F, 0x0E, 0x0D, 0x0C
                );

                while (array.Length >= 16)
                {
                    var vec = Vector128.ShuffleNative(Vector128.LoadUnsafe(ref array[0]), shuffler);
                    vec.StoreUnsafe(ref array[0]);
                    array = array[16..];
                }
            }

            array.Swap32NoSimd();
        }

        public void Swap32NoSimd()
        {
            for (var i = 0; i < array.Length - 3; i += 4)
                WriteInt32LittleEndian(array[i..], ReadInt32BigEndian(array[i..]));
        }
        
        public void Swap64()
        {
            if (Vector512.IsHardwareAccelerated && array.Length > 64)
            {
                var shuffler = Vector512.Create(
                    (byte)0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x00,
                    0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09, 0x08,
                    0x17, 0x16, 0x15, 0x14, 0x13, 0x12, 0x11, 0x10,
                    0x1F, 0x1E, 0x1D, 0x1C, 0x1B, 0x1A, 0x19, 0x18,
                    0x27, 0x26, 0x25, 0x24, 0x23, 0x22, 0x21, 0x20,
                    0x2F, 0x2E, 0x2D, 0x2C, 0x2B, 0x2A, 0x29, 0x28,
                    0x37, 0x36, 0x35, 0x34, 0x33, 0x32, 0x31, 0x30,
                    0x3F, 0x3E, 0x3D, 0x3C, 0x3B, 0x3A, 0x39, 0x38
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
                    (byte)0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x00,
                    0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09, 0x08,
                    0x17, 0x16, 0x15, 0x14, 0x13, 0x12, 0x11, 0x10,
                    0x1F, 0x1E, 0x1D, 0x1C, 0x1B, 0x1A, 0x19, 0x18
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
                    (byte)0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x00,
                    0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09, 0x08
                );

                while (array.Length >= 16)
                {
                    var vec = Vector128.ShuffleNative(Vector128.LoadUnsafe(ref array[0]), shuffler);
                    vec.StoreUnsafe(ref array[0]);
                    array = array[16..];
                }
            }

            array.Swap64NoSimd();
        }

        public void Swap64NoSimd()
        {
            for (var i = 0; i < array.Length - 7; i += 8)
                WriteInt64LittleEndian(array[i..], ReadInt64BigEndian(array[i..]));
        }
    }
}