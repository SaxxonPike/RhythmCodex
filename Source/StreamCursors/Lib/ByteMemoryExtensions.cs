using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Saxxon.StreamCursors;

[PublicAPI]
public static class ByteMemoryExtensions
{
    // ***************************************************

    #region ReadOnlyMemory

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadU8(
        this ReadOnlyMemory<byte> memory,
        out byte val)
    {
        val = memory.Span[0];
        return memory[1..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadS8(
        this ReadOnlyMemory<byte> memory,
        out sbyte val)
    {
        val = MemoryMarshal.Cast<byte, sbyte>(memory.Span)[0];
        return memory[1..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadU16L(
        this ReadOnlyMemory<byte> memory,
        out ushort val)
    {
        val = BinaryPrimitives.ReadUInt16LittleEndian(memory.Span);
        return memory[2..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadS16L(
        this ReadOnlyMemory<byte> memory,
        out short val)
    {
        val = BinaryPrimitives.ReadInt16LittleEndian(memory.Span);
        return memory[2..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadU16B(
        this ReadOnlyMemory<byte> memory,
        out ushort val)
    {
        val = BinaryPrimitives.ReadUInt16BigEndian(memory.Span);
        return memory[2..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadS16B(
        this ReadOnlyMemory<byte> memory,
        out short val)
    {
        val = BinaryPrimitives.ReadInt16BigEndian(memory.Span);
        return memory[2..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadU32L(
        this ReadOnlyMemory<byte> memory,
        out uint val)
    {
        val = BinaryPrimitives.ReadUInt32LittleEndian(memory.Span);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadS32L(
        this ReadOnlyMemory<byte> memory,
        out int val)
    {
        val = BinaryPrimitives.ReadInt32LittleEndian(memory.Span);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadU32B(
        this ReadOnlyMemory<byte> memory,
        out uint val)
    {
        val = BinaryPrimitives.ReadUInt32BigEndian(memory.Span);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadS32B(
        this ReadOnlyMemory<byte> memory,
        out int val)
    {
        val = BinaryPrimitives.ReadInt32BigEndian(memory.Span);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadF32L(
        this ReadOnlyMemory<byte> memory,
        out float val)
    {
        val = BinaryPrimitives.ReadSingleLittleEndian(memory.Span);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadF32B(
        this ReadOnlyMemory<byte> memory,
        out float val)
    {
        val = BinaryPrimitives.ReadSingleBigEndian(memory.Span);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadU64L(
        this ReadOnlyMemory<byte> memory,
        out ulong val)
    {
        val = BinaryPrimitives.ReadUInt64LittleEndian(memory.Span);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadS64L(
        this ReadOnlyMemory<byte> memory,
        out long val)
    {
        val = BinaryPrimitives.ReadInt64LittleEndian(memory.Span);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadU64B(
        this ReadOnlyMemory<byte> memory,
        out ulong val)
    {
        val = BinaryPrimitives.ReadUInt64BigEndian(memory.Span);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadS64B(
        this ReadOnlyMemory<byte> memory,
        out long val)
    {
        val = BinaryPrimitives.ReadInt64BigEndian(memory.Span);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadF64L(
        this ReadOnlyMemory<byte> memory,
        out double val)
    {
        val = BinaryPrimitives.ReadDoubleLittleEndian(memory.Span);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> ReadF64B(
        this ReadOnlyMemory<byte> memory,
        out double val)
    {
        val = BinaryPrimitives.ReadDoubleBigEndian(memory.Span);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> Skip(
        this ReadOnlyMemory<byte> memory,
        [NonNegativeValue] int count)
    {
        return memory[count..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> Extract(
        this ReadOnlyMemory<byte> memory,
        [NonNegativeValue] int count,
        out ReadOnlyMemory<byte> val)
    {
        val = memory[..count];
        return memory[count..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> TryExtract(
        this ReadOnlyMemory<byte> memory,
        int count,
        out ReadOnlyMemory<byte> val)
    {
        val = memory[..Math.Min(memory.Length, count)];
        return memory[val.Length..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> Read(
        this ReadOnlyMemory<byte> memory,
        [NonNegativeValue] int count,
        out byte[] val)
    {
        val = memory[..count].ToArray();
        return memory[count..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> Read(
        this ReadOnlyMemory<byte> memory,
        Span<byte> target,
        [NonNegativeValue] out int count)
    {
        count = target.Length;
        memory[..count].Span.CopyTo(target);
        return memory[count..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> Read(
        this ReadOnlyMemory<byte> memory,
        Memory<byte> target,
        [NonNegativeValue] out int count)
    {
        count = target.Length;
        memory[..count].CopyTo(target);
        return memory[count..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> TryRead(
        this ReadOnlyMemory<byte> memory,
        [NonNegativeValue] int count,
        out byte[] val)
    {
        val = memory[..Math.Min(memory.Length, count)].ToArray();
        return memory[val.Length..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> TryRead(
        this ReadOnlyMemory<byte> memory,
        Span<byte> target,
        [NonNegativeValue] out int count)
    {
        count = target.Length;
        memory[..Math.Min(memory.Length, count)].Span.CopyTo(target);
        return memory[count..];
    }

    [DebuggerStepThrough]
    public static ReadOnlyMemory<byte> TryRead(
        this ReadOnlyMemory<byte> memory,
        Memory<byte> target,
        [NonNegativeValue] out int count)
    {
        count = target.Length;
        memory[..Math.Min(memory.Length, count)].CopyTo(target);
        return memory[count..];
    }

    #endregion

    // ***************************************************

    #region Memory

    [DebuggerStepThrough]
    public static Memory<byte> ReadU8(
        this Memory<byte> memory,
        out byte val)
    {
        val = memory.Span[0];
        return memory[1..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteU8(
        this Memory<byte> memory,
        byte val)
    {
        memory.Span[0] = val;
        return memory[1..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadS8(
        this Memory<byte> memory,
        out sbyte val)
    {
        val = MemoryMarshal.Cast<byte, sbyte>(memory.Span)[0];
        return memory[1..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteS8(
        this Memory<byte> memory,
        sbyte val)
    {
        MemoryMarshal.Cast<byte, sbyte>(memory.Span)[0] = val;
        return memory[1..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadU16L(
        this Memory<byte> memory,
        out ushort val)
    {
        val = BinaryPrimitives.ReadUInt16LittleEndian(memory.Span);
        return memory[2..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteU16L(
        this Memory<byte> memory,
        ushort val)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(memory.Span, val);
        return memory[2..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadS16L(
        this Memory<byte> memory,
        out short val)
    {
        val = BinaryPrimitives.ReadInt16LittleEndian(memory.Span);
        return memory[2..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteS16L(
        this Memory<byte> memory,
        short val)
    {
        BinaryPrimitives.WriteInt16LittleEndian(memory.Span, val);
        return memory[2..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadU16B(
        this Memory<byte> memory,
        out ushort val)
    {
        val = BinaryPrimitives.ReadUInt16BigEndian(memory.Span);
        return memory[2..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteU16B(
        this Memory<byte> memory,
        ushort val)
    {
        BinaryPrimitives.WriteUInt16BigEndian(memory.Span, val);
        return memory[2..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadS16B(
        this Memory<byte> memory,
        out short val)
    {
        val = BinaryPrimitives.ReadInt16BigEndian(memory.Span);
        return memory[2..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteS16B(
        this Memory<byte> memory,
        short val)
    {
        BinaryPrimitives.WriteInt16BigEndian(memory.Span, val);
        return memory[2..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadU32L(
        this Memory<byte> memory,
        out uint val)
    {
        val = BinaryPrimitives.ReadUInt32LittleEndian(memory.Span);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteU32L(
        this Memory<byte> memory,
        uint val)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(memory.Span, val);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadS32L(
        this Memory<byte> memory,
        out int val)
    {
        val = BinaryPrimitives.ReadInt32LittleEndian(memory.Span);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteS32L(
        this Memory<byte> memory,
        int val)
    {
        BinaryPrimitives.WriteInt32LittleEndian(memory.Span, val);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadU32B(
        this Memory<byte> memory,
        out uint val)
    {
        val = BinaryPrimitives.ReadUInt32BigEndian(memory.Span);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteU32B(
        this Memory<byte> memory,
        uint val)
    {
        BinaryPrimitives.WriteUInt32BigEndian(memory.Span, val);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadS32B(
        this Memory<byte> memory,
        out int val)
    {
        val = BinaryPrimitives.ReadInt32BigEndian(memory.Span);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteS32B(
        this Memory<byte> memory,
        int val)
    {
        BinaryPrimitives.WriteInt32BigEndian(memory.Span, val);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadF32L(
        this Memory<byte> memory,
        out float val)
    {
        val = BinaryPrimitives.ReadSingleLittleEndian(memory.Span);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteF32L(
        this Memory<byte> memory,
        float val)
    {
        BinaryPrimitives.WriteSingleLittleEndian(memory.Span, val);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadF32B(
        this Memory<byte> memory,
        out float val)
    {
        val = BinaryPrimitives.ReadSingleBigEndian(memory.Span);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteF32B(
        this Memory<byte> memory,
        float val)
    {
        BinaryPrimitives.WriteSingleBigEndian(memory.Span, val);
        return memory[4..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadU64L(
        this Memory<byte> memory,
        out ulong val)
    {
        val = BinaryPrimitives.ReadUInt64LittleEndian(memory.Span);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteU64L(
        this Memory<byte> memory,
        ulong val)
    {
        BinaryPrimitives.WriteUInt64LittleEndian(memory.Span, val);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadS64L(
        this Memory<byte> memory,
        out long val)
    {
        val = BinaryPrimitives.ReadInt64LittleEndian(memory.Span);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteS64L(
        this Memory<byte> memory,
        long val)
    {
        BinaryPrimitives.WriteInt64LittleEndian(memory.Span, val);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadU64B(
        this Memory<byte> memory,
        out ulong val)
    {
        val = BinaryPrimitives.ReadUInt64BigEndian(memory.Span);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteU64B(
        this Memory<byte> memory,
        ulong val)
    {
        BinaryPrimitives.WriteUInt64BigEndian(memory.Span, val);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadS64B(
        this Memory<byte> memory,
        out long val)
    {
        val = BinaryPrimitives.ReadInt64BigEndian(memory.Span);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteS64B(
        this Memory<byte> memory,
        long val)
    {
        BinaryPrimitives.WriteInt64BigEndian(memory.Span, val);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadF64L(
        this Memory<byte> memory,
        out double val)
    {
        val = BinaryPrimitives.ReadDoubleLittleEndian(memory.Span);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteF64L(
        this Memory<byte> memory,
        double val)
    {
        BinaryPrimitives.WriteDoubleLittleEndian(memory.Span, val);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> ReadF64B(
        this Memory<byte> memory,
        out double val)
    {
        val = BinaryPrimitives.ReadDoubleBigEndian(memory.Span);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> WriteF64B(
        this Memory<byte> memory,
        double val)
    {
        BinaryPrimitives.WriteDoubleBigEndian(memory.Span, val);
        return memory[8..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> Skip(
        this Memory<byte> memory,
        [NonNegativeValue] int count)
    {
        return memory[count..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> Extract(
        this Memory<byte> memory,
        [NonNegativeValue] int count,
        out Memory<byte> val)
    {
        val = memory[..count];
        return memory[count..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> TryExtract(
        this Memory<byte> memory,
        int count,
        out Memory<byte> val)
    {
        val = memory[..Math.Min(memory.Length, count)];
        return memory[val.Length..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> Read(
        this Memory<byte> memory,
        [NonNegativeValue] int count,
        out byte[] val)
    {
        val = memory[..count].ToArray();
        return memory[count..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> Read(
        this Memory<byte> memory,
        Span<byte> target,
        [NonNegativeValue] out int count)
    {
        count = target.Length;
        memory[..count].Span.CopyTo(target);
        return memory[count..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> Read(
        this Memory<byte> memory,
        Memory<byte> target,
        [NonNegativeValue] out int count)
    {
        count = target.Length;
        memory[..count].CopyTo(target);
        return memory[count..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> TryRead(
        this Memory<byte> memory,
        [NonNegativeValue] int count,
        out byte[] val)
    {
        val = memory[..Math.Min(memory.Length, count)].ToArray();
        return memory[val.Length..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> TryRead(
        this Memory<byte> memory,
        Span<byte> target,
        [NonNegativeValue] out int count)
    {
        count = target.Length;
        memory[..Math.Min(memory.Length, count)].Span.CopyTo(target);
        return memory[count..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> TryRead(
        this Memory<byte> memory,
        Memory<byte> target,
        [NonNegativeValue] out int count)
    {
        count = target.Length;
        memory[..Math.Min(memory.Length, count)].CopyTo(target);
        return memory[count..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> Write(
        this Memory<byte> memory,
        ReadOnlySpan<byte> val,
        out int count)
    {
        count = val.Length;
        val.CopyTo(memory.Span);
        return memory[count..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> Write(
        this Memory<byte> memory,
        ReadOnlyMemory<byte> val,
        out int count)
    {
        count = val.Length;
        val.CopyTo(memory);
        return memory[count..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> TryWrite(
        this Memory<byte> memory,
        ReadOnlySpan<byte> val,
        out int count)
    {
        count = Math.Min(memory.Length, val.Length);
        val[..count].CopyTo(memory.Span);
        return memory[count..];
    }

    [DebuggerStepThrough]
    public static Memory<byte> TryWrite(
        this Memory<byte> memory,
        ReadOnlyMemory<byte> val,
        out int count)
    {
        count = Math.Min(memory.Length, val.Length);
        val[..count].CopyTo(memory);
        return memory[count..];
    }

    #endregion

    // ***************************************************
}