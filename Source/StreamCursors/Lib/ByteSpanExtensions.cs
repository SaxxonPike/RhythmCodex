using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Saxxon.StreamCursors;

[PublicAPI]
public static class ByteSpanExtensions
{
    // ***************************************************

    #region ReadOnlySpan

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadU8(
        this ReadOnlySpan<byte> span,
        out byte val)
    {
        val = span[0];
        return span[1..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadS8(
        this ReadOnlySpan<byte> span,
        out sbyte val)
    {
        val = MemoryMarshal.Cast<byte, sbyte>(span)[0];
        return span[1..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadU16L(
        this ReadOnlySpan<byte> span,
        out ushort val)
    {
        val = BinaryPrimitives.ReadUInt16LittleEndian(span);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadS16L(
        this ReadOnlySpan<byte> span,
        out short val)
    {
        val = BinaryPrimitives.ReadInt16LittleEndian(span);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadU16B(
        this ReadOnlySpan<byte> span,
        out ushort val)
    {
        val = BinaryPrimitives.ReadUInt16BigEndian(span);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadS16B(
        this ReadOnlySpan<byte> span,
        out short val)
    {
        val = BinaryPrimitives.ReadInt16BigEndian(span);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadU32L(
        this ReadOnlySpan<byte> span,
        out uint val)
    {
        val = BinaryPrimitives.ReadUInt32LittleEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadS32L(
        this ReadOnlySpan<byte> span,
        out int val)
    {
        val = BinaryPrimitives.ReadInt32LittleEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadU32B(
        this ReadOnlySpan<byte> span,
        out uint val)
    {
        val = BinaryPrimitives.ReadUInt32BigEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadS32B(
        this ReadOnlySpan<byte> span,
        out int val)
    {
        val = BinaryPrimitives.ReadInt32BigEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadF32L(
        this ReadOnlySpan<byte> span,
        out float val)
    {
        val = BinaryPrimitives.ReadSingleLittleEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadF32B(
        this ReadOnlySpan<byte> span,
        out float val)
    {
        val = BinaryPrimitives.ReadSingleBigEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadU64L(
        this ReadOnlySpan<byte> span,
        out ulong val)
    {
        val = BinaryPrimitives.ReadUInt64LittleEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadS64L(
        this ReadOnlySpan<byte> span,
        out long val)
    {
        val = BinaryPrimitives.ReadInt64LittleEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadU64B(
        this ReadOnlySpan<byte> span,
        out ulong val)
    {
        val = BinaryPrimitives.ReadUInt64BigEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadS64B(
        this ReadOnlySpan<byte> span,
        out long val)
    {
        val = BinaryPrimitives.ReadInt64BigEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadF64L(
        this ReadOnlySpan<byte> span,
        out double val)
    {
        val = BinaryPrimitives.ReadDoubleLittleEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadF64B(
        this ReadOnlySpan<byte> span,
        out double val)
    {
        val = BinaryPrimitives.ReadDoubleBigEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> Skip(
        this ReadOnlySpan<byte> span,
        [NonNegativeValue] int count)
    {
        return span[count..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> Extract(
        this ReadOnlySpan<byte> span,
        [NonNegativeValue] int count,
        out ReadOnlySpan<byte> val)
    {
        val = span[..count];
        return span[count..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> TryExtract(
        this ReadOnlySpan<byte> span,
        int count,
        out ReadOnlySpan<byte> val)
    {
        val = span[..Math.Min(span.Length, count)];
        return span[val.Length..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> Read(
        this ReadOnlySpan<byte> span,
        [NonNegativeValue] int count,
        out byte[] val)
    {
        val = span[..count].ToArray();
        return span[count..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> Read(
        this ReadOnlySpan<byte> span,
        Memory<byte> target,
        [NonNegativeValue] out int count)
    {
        count = target.Length;
        span[..count].CopyTo(target.Span);
        return span[count..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> Read(
        this ReadOnlySpan<byte> span,
        Span<byte> target,
        [NonNegativeValue] out int count)
    {
        count = target.Length;
        span[..count].CopyTo(target);
        return span[count..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> TryRead(
        this ReadOnlySpan<byte> span,
        [NonNegativeValue] int count,
        out byte[] val)
    {
        val = span[..Math.Min(span.Length, count)].ToArray();
        return span[val.Length..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> TryRead(
        this ReadOnlySpan<byte> span,
        Memory<byte> target,
        [NonNegativeValue] out int count)
    {
        count = target.Length;
        span[..Math.Min(span.Length, count)].CopyTo(target.Span);
        return span[count..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> TryRead(
        this ReadOnlySpan<byte> span,
        Span<byte> target,
        [NonNegativeValue] out int count)
    {
        count = target.Length;
        span[..Math.Min(span.Length, count)].CopyTo(target);
        return span[count..];
    }
    
    #endregion

    // ***************************************************

    #region Span

    [DebuggerStepThrough]
    public static Span<byte> ReadU8(
        this Span<byte> span,
        out byte val)
    {
        val = span[0];
        return span[1..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteU8(
        this Span<byte> span,
        byte val)
    {
        span[0] = val;
        return span[1..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadS8(
        this Span<byte> span,
        out sbyte val)
    {
        val = MemoryMarshal.Cast<byte, sbyte>(span)[0];
        return span[1..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteS8(
        this Span<byte> span,
        sbyte val)
    {
        MemoryMarshal.Cast<byte, sbyte>(span)[0] = val;
        return span[1..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadU16L(
        this Span<byte> span,
        out ushort val)
    {
        val = BinaryPrimitives.ReadUInt16LittleEndian(span);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteU16L(
        this Span<byte> span,
        ushort val)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(span, val);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadS16L(
        this Span<byte> span,
        out short val)
    {
        val = BinaryPrimitives.ReadInt16LittleEndian(span);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteS16L(
        this Span<byte> span,
        short val)
    {
        BinaryPrimitives.WriteInt16LittleEndian(span, val);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadU16B(
        this Span<byte> span,
        out ushort val)
    {
        val = BinaryPrimitives.ReadUInt16BigEndian(span);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteU16B(
        this Span<byte> span,
        ushort val)
    {
        BinaryPrimitives.WriteUInt16BigEndian(span, val);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadS16B(
        this Span<byte> span,
        out short val)
    {
        val = BinaryPrimitives.ReadInt16BigEndian(span);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteS16B(
        this Span<byte> span,
        short val)
    {
        BinaryPrimitives.WriteInt16BigEndian(span, val);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadU32L(
        this Span<byte> span,
        out uint val)
    {
        val = BinaryPrimitives.ReadUInt32LittleEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteU32L(
        this Span<byte> span,
        uint val)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(span, val);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadS32L(
        this Span<byte> span,
        out int val)
    {
        val = BinaryPrimitives.ReadInt32LittleEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteS32L(
        this Span<byte> span,
        int val)
    {
        BinaryPrimitives.WriteInt32LittleEndian(span, val);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadU32B(
        this Span<byte> span,
        out uint val)
    {
        val = BinaryPrimitives.ReadUInt32BigEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteU32B(
        this Span<byte> span,
        uint val)
    {
        BinaryPrimitives.WriteUInt32BigEndian(span, val);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadS32B(
        this Span<byte> span,
        out int val)
    {
        val = BinaryPrimitives.ReadInt32BigEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteS32B(
        this Span<byte> span,
        int val)
    {
        BinaryPrimitives.WriteInt32BigEndian(span, val);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadF32L(
        this Span<byte> span,
        out float val)
    {
        val = BinaryPrimitives.ReadSingleLittleEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteF32L(
        this Span<byte> span,
        float val)
    {
        BinaryPrimitives.WriteSingleLittleEndian(span, val);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadF32B(
        this Span<byte> span,
        out float val)
    {
        val = BinaryPrimitives.ReadSingleBigEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteF32B(
        this Span<byte> span,
        float val)
    {
        BinaryPrimitives.WriteSingleBigEndian(span, val);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadU64L(
        this Span<byte> span,
        out ulong val)
    {
        val = BinaryPrimitives.ReadUInt64LittleEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteU64L(
        this Span<byte> span,
        ulong val)
    {
        BinaryPrimitives.WriteUInt64LittleEndian(span, val);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadS64L(
        this Span<byte> span,
        out long val)
    {
        val = BinaryPrimitives.ReadInt64LittleEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteS64L(
        this Span<byte> span,
        long val)
    {
        BinaryPrimitives.WriteInt64LittleEndian(span, val);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadU64B(
        this Span<byte> span,
        out ulong val)
    {
        val = BinaryPrimitives.ReadUInt64BigEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteU64B(
        this Span<byte> span,
        ulong val)
    {
        BinaryPrimitives.WriteUInt64BigEndian(span, val);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadS64B(
        this Span<byte> span,
        out long val)
    {
        val = BinaryPrimitives.ReadInt64BigEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteS64B(
        this Span<byte> span,
        long val)
    {
        BinaryPrimitives.WriteInt64BigEndian(span, val);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadF64L(
        this Span<byte> span,
        out double val)
    {
        val = BinaryPrimitives.ReadDoubleLittleEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteF64L(
        this Span<byte> span,
        double val)
    {
        BinaryPrimitives.WriteDoubleLittleEndian(span, val);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadF64B(
        this Span<byte> span,
        out double val)
    {
        val = BinaryPrimitives.ReadDoubleBigEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteF64B(
        this Span<byte> span,
        double val)
    {
        BinaryPrimitives.WriteDoubleBigEndian(span, val);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> Skip(
        this Span<byte> span,
        [NonNegativeValue] int count)
    {
        return span[count..];
    }

    [DebuggerStepThrough]
    public static Span<byte> Extract(
        this Span<byte> span,
        [NonNegativeValue] int count,
        out Span<byte> val)
    {
        val = span[..count];
        return span[count..];
    }
    
    [DebuggerStepThrough]
    public static Span<byte> TryExtract(
        this Span<byte> span,
        int count,
        out Span<byte> val)
    {
        val = span[..Math.Min(span.Length, count)];
        return span[val.Length..];
    }

    [DebuggerStepThrough]
    public static Span<byte> Read(
        this Span<byte> span,
        [NonNegativeValue] int count,
        out byte[] val)
    {
        val = span[..count].ToArray();
        return span[count..];
    }

    [DebuggerStepThrough]
    public static Span<byte> Read(
        this Span<byte> span,
        Span<byte> target,
        [NonNegativeValue] out int count)
    {
        count = target.Length;
        span[..count].CopyTo(target);
        return span[count..];
    }

    [DebuggerStepThrough]
    public static Span<byte> Read(
        this Span<byte> span,
        Memory<byte> target,
        [NonNegativeValue] out int count)
    {
        count = target.Length;
        span[..count].CopyTo(target.Span);
        return span[count..];
    }

    [DebuggerStepThrough]
    public static Span<byte> TryRead(
        this Span<byte> span,
        [NonNegativeValue] int count,
        out byte[] val)
    {
        val = span[..Math.Min(span.Length, count)].ToArray();
        return span[val.Length..];
    }

    [DebuggerStepThrough]
    public static Span<byte> TryRead(
        this Span<byte> span,
        Span<byte> target,
        [NonNegativeValue] out int count)
    {
        count = Math.Min(span.Length, target.Length);
        span[..count].CopyTo(target);
        return span[count..];
    }

    [DebuggerStepThrough]
    public static Span<byte> TryRead(
        this Span<byte> span,
        Memory<byte> target,
        [NonNegativeValue] out int count)
    {
        count = target.Length;
        span[..Math.Min(span.Length, count)].CopyTo(target.Span);
        return span[count..];
    }

    [DebuggerStepThrough]
    public static Span<byte> Write(
        this Span<byte> span,
        ReadOnlySpan<byte> val,
        out int count)
    {
        count = val.Length;
        val.CopyTo(span);
        return span[count..];
    }

    [DebuggerStepThrough]
    public static Span<byte> Write(
        this Span<byte> span,
        ReadOnlyMemory<byte> val,
        out int count)
    {
        count = val.Length;
        val.Span.CopyTo(span);
        return span[count..];
    }

    [DebuggerStepThrough]
    public static Span<byte> TryWrite(
        this Span<byte> span,
        ReadOnlyMemory<byte> val,
        out int count)
    {
        count = Math.Min(span.Length, val.Length);
        val[..count].Span.CopyTo(span);
        return span[count..];
    }

    [DebuggerStepThrough]
    public static Span<byte> TryWrite(
        this Span<byte> span,
        ReadOnlySpan<byte> val,
        out int count)
    {
        count = Math.Min(span.Length, val.Length);
        val[..count].CopyTo(span);
        return span[count..];
    }

    #endregion

    // ***************************************************
}