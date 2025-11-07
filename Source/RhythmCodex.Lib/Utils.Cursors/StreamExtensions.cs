using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;

namespace RhythmCodex.Utils.Cursors;

[PublicAPI]
public static class StreamExtensions
{
    [DebuggerStepThrough]
    public static byte[] Read(
        this Stream stream,
        [NonNegativeValue] int count)
    {
        var data = new byte[count];
        var actual = stream.ReadAtLeast(data, count, false);
        if (actual < data.Length)
            Array.Resize(ref data, actual);
        return data;
    }

    [DebuggerStepThrough]
    public static Stream ReadU8(
        this Stream stream,
        out byte val)
    {
        Span<byte> buffer = stackalloc byte[1];
        stream.ReadExactly(buffer);
        val = buffer[0];
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteU8(
        this Stream stream,
        byte val)
    {
        Span<byte> buffer = stackalloc byte[1];
        buffer[0] = val;
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadS8(
        this Stream stream,
        out sbyte val)
    {
        Span<byte> buffer = stackalloc byte[1];
        stream.ReadExactly(buffer);
        val = unchecked((sbyte)buffer[0]);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteS8(
        this Stream stream,
        sbyte val)
    {
        Span<byte> buffer = stackalloc byte[1];
        buffer[0] = unchecked((byte)val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadU16L(
        this Stream stream,
        out ushort val)
    {
        Span<byte> buffer = stackalloc byte[2];
        stream.ReadExactly(buffer);
        val = BinaryPrimitives.ReadUInt16LittleEndian(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteU16L(
        this Stream stream,
        ushort val)
    {
        Span<byte> buffer = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadU16B(
        this Stream stream,
        out ushort val)
    {
        Span<byte> buffer = stackalloc byte[2];
        stream.ReadExactly(buffer);
        val = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteU16B(
        this Stream stream,
        ushort val)
    {
        Span<byte> buffer = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadS16L(
        this Stream stream,
        out short val)
    {
        Span<byte> buffer = stackalloc byte[2];
        stream.ReadExactly(buffer);
        val = BinaryPrimitives.ReadInt16LittleEndian(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteS16L(
        this Stream stream,
        short val)
    {
        Span<byte> buffer = stackalloc byte[2];
        BinaryPrimitives.WriteInt16LittleEndian(buffer, val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadS16B(
        this Stream stream,
        out short val)
    {
        Span<byte> buffer = stackalloc byte[2];
        stream.ReadExactly(buffer);
        val = BinaryPrimitives.ReadInt16BigEndian(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteS16B(
        this Stream stream,
        short val)
    {
        Span<byte> buffer = stackalloc byte[2];
        BinaryPrimitives.WriteInt16BigEndian(buffer, val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadU32L(
        this Stream stream,
        out uint val)
    {
        Span<byte> buffer = stackalloc byte[4];
        stream.ReadExactly(buffer);
        val = BinaryPrimitives.ReadUInt32LittleEndian(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteU32L(
        this Stream stream,
        uint val)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(buffer, val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadU32B(
        this Stream stream,
        out uint val)
    {
        Span<byte> buffer = stackalloc byte[4];
        stream.ReadExactly(buffer);
        val = BinaryPrimitives.ReadUInt32BigEndian(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteU32B(
        this Stream stream,
        uint val)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(buffer, val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadS32L(
        this Stream stream,
        out int val)
    {
        Span<byte> buffer = stackalloc byte[4];
        stream.ReadExactly(buffer);
        val = BinaryPrimitives.ReadInt32LittleEndian(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteS32L(
        this Stream stream,
        int val)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(buffer, val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadS32B(
        this Stream stream,
        out int val)
    {
        Span<byte> buffer = stackalloc byte[4];
        stream.ReadExactly(buffer);
        val = BinaryPrimitives.ReadInt32BigEndian(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteS32B(
        this Stream stream,
        int val)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(buffer, val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadF32L(
        this Stream stream,
        out float val)
    {
        Span<byte> buffer = stackalloc byte[4];
        stream.ReadExactly(buffer);
        val = BinaryPrimitives.ReadSingleLittleEndian(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteF32L(
        this Stream stream,
        float val)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteSingleLittleEndian(buffer, val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadF32B(
        this Stream stream,
        out float val)
    {
        Span<byte> buffer = stackalloc byte[4];
        stream.ReadExactly(buffer);
        val = BinaryPrimitives.ReadSingleBigEndian(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteF32B(
        this Stream stream,
        float val)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteSingleBigEndian(buffer, val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadU64L(
        this Stream stream,
        out ulong val)
    {
        Span<byte> buffer = stackalloc byte[8];
        stream.ReadExactly(buffer);
        val = BinaryPrimitives.ReadUInt64LittleEndian(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteU64L(
        this Stream stream,
        ulong val)
    {
        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteUInt64LittleEndian(buffer, val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadU64B(
        this Stream stream,
        out ulong val)
    {
        Span<byte> buffer = stackalloc byte[8];
        stream.ReadExactly(buffer);
        val = BinaryPrimitives.ReadUInt64BigEndian(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteU64B(
        this Stream stream,
        ulong val)
    {
        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteUInt64BigEndian(buffer, val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadS64L(
        this Stream stream,
        out long val)
    {
        Span<byte> buffer = stackalloc byte[8];
        stream.ReadExactly(buffer);
        val = BinaryPrimitives.ReadInt64LittleEndian(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteS64L(
        this Stream stream,
        long val)
    {
        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteInt64LittleEndian(buffer, val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadS64B(
        this Stream stream,
        out long val)
    {
        Span<byte> buffer = stackalloc byte[8];
        stream.ReadExactly(buffer);
        val = BinaryPrimitives.ReadInt64BigEndian(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteS64B(
        this Stream stream,
        long val)
    {
        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteInt64BigEndian(buffer, val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadF64L(
        this Stream stream,
        out double val)
    {
        Span<byte> buffer = stackalloc byte[8];
        stream.ReadExactly(buffer);
        val = BinaryPrimitives.ReadDoubleLittleEndian(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteF64L(
        this Stream stream,
        double val)
    {
        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteDoubleLittleEndian(buffer, val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream ReadF64B(
        this Stream stream,
        out double val)
    {
        Span<byte> buffer = stackalloc byte[8];
        stream.ReadExactly(buffer);
        val = BinaryPrimitives.ReadDoubleBigEndian(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream WriteF64B(
        this Stream stream,
        double val)
    {
        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteDoubleBigEndian(buffer, val);
        stream.Write(buffer);
        return stream;
    }

    [DebuggerStepThrough]
    public static Stream Skip(
        this Stream stream,
        [NonNegativeValue] int count)
    {
        if (stream.CanSeek)
        {
            stream.Seek(count, SeekOrigin.Current);
            return stream;
        }

        switch (count)
        {
            case 0:
                return stream;
            case <= 256:
            {
                Span<byte> buffer = stackalloc byte[count];
                stream.ReadAtLeast(buffer, count, false);
                return stream;
            }
            default:
            {
                using var mem = MemoryPool<byte>.Shared.Rent();
                var span = mem.Memory.Span;
                var remaining = count;
                while (remaining > 0)
                {
                    var amount = stream.Read(span[..Math.Max(span.Length, remaining)]);
                    if (amount < 1)
                        break;
                    remaining -= amount;
                }

                return stream;
            }
        }
    }
}