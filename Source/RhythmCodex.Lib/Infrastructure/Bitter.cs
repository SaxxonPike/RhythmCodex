using System;
using System.Buffers.Binary;

namespace RhythmCodex.Infrastructure
{
    public static class Bitter
    {
        public static int[] ToInt16Values(ReadOnlySpan<byte> span, int offset, int count)
        {
            var output = new int[count];
            for (var i = 0; i < count; i++)
                output[i] = ToInt16(span, offset + i * 2);
            return output;
        }

        public static short ToInt16(ReadOnlySpan<byte> span) =>
            BinaryPrimitives.ReadInt16LittleEndian(span);

        public static short ToInt16(ReadOnlySpan<byte> span, int offset) =>
            BinaryPrimitives.ReadInt16LittleEndian(span[offset..]);

        public static short ToInt16(byte lsb, byte msb) =>
            unchecked((short)(lsb | (msb << 8)));

        public static short ToInt16S(ReadOnlySpan<byte> span) =>
            BinaryPrimitives.ReadInt16BigEndian(span);

        public static short ToInt16S(ReadOnlySpan<byte> span, int offset) =>
            BinaryPrimitives.ReadInt16BigEndian(span[offset..]);

        public static int ToInt24(ReadOnlySpan<byte> span) =>
            ToInt24(span[0], span[1], span[2]);

        public static int ToInt24(ReadOnlySpan<byte> span, int offset) =>
            ToInt24(span[offset..]);

        public static int ToInt24(byte lsb, byte mid, byte msb) =>
            ((lsb << 8) | (mid << 16) | (msb << 24)) >> 8;

        public static int ToInt24S(ReadOnlySpan<byte> span) =>
            ToInt24(span[2], span[1], span[0]);

        public static int ToInt24S(ReadOnlySpan<byte> span, int offset) =>
            ToInt24S(span[offset..]);

        public static int ToInt32(ReadOnlySpan<byte> span) =>
            BinaryPrimitives.ReadInt32LittleEndian(span);

        public static int ToInt32(ReadOnlySpan<byte> span, int offset) =>
            BinaryPrimitives.ReadInt32LittleEndian(span[offset..]);

        public static int ToInt32(byte lsb, byte b, byte c, byte msb) =>
            lsb | (b << 8) | (c << 16) | (msb << 24);

        public static int ToInt32S(ReadOnlySpan<byte> span) =>
            BinaryPrimitives.ReadInt32BigEndian(span);

        public static int ToInt32S(ReadOnlySpan<byte> span, int offset) =>
            BinaryPrimitives.ReadInt32BigEndian(span[offset..]);

        public static float ToFloat(ReadOnlySpan<byte> span) =>
            BinaryPrimitives.ReadSingleLittleEndian(span);
        
        public static float ToFloat(ReadOnlySpan<byte> span, int offset) =>
            BinaryPrimitives.ReadSingleLittleEndian(span[offset..]);
        
        public static int[] ToInt16Values(ReadOnlySpan<byte> bytes)
        {
            var result = new int[bytes.Length / sizeof(short)];
            for (int i = 0, j = 0; j < bytes.Length; i++, j += sizeof(short))
                result[i] = BinaryPrimitives.ReadInt16LittleEndian(bytes[j..]);
            return result;
        }

        public static void ToInt16Values(ReadOnlySpan<byte> bytes, Span<short> result)
        {
            for (int i = 0, j = 0; j < bytes.Length; i++, j += sizeof(short))
                result[i] = BinaryPrimitives.ReadInt16LittleEndian(bytes[j..]);
        }

        public static int[] ToInt32Values(ReadOnlySpan<byte> bytes)
        {
            var result = new int[bytes.Length / sizeof(int)];
            for (int i = 0, j = 0; j < bytes.Length; i++, j += sizeof(int))
                result[i] = BinaryPrimitives.ReadInt32LittleEndian(bytes[j..]);
            return result;
        }
    
        public static void ToInt32Values(ReadOnlySpan<byte> bytes, Span<int> result)
        {
            for (int i = 0, j = 0; j < bytes.Length; i++, j += sizeof(int))
                result[i] = BinaryPrimitives.ReadInt32LittleEndian(bytes[j..]);
        }
    }
}