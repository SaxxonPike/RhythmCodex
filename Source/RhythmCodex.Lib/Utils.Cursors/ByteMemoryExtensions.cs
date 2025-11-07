using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace RhythmCodex.Utils.Cursors;

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

    #region Conversions

    [DebuggerStepThrough]
    public static Memory<ushort> ToU16L(this ReadOnlyMemory<byte> mem) =>
        mem.Span.ToU16L();

    [DebuggerStepThrough]
    public static Memory<ushort> ToU16L(this Memory<byte> mem) =>
        mem.Span.ToU16L();

    [DebuggerStepThrough]
    public static Memory<ushort> ToU16B(this ReadOnlyMemory<byte> mem) =>
        mem.Span.ToU16B();

    [DebuggerStepThrough]
    public static Memory<ushort> ToU16B(this Memory<byte> mem) =>
        mem.Span.ToU16B();

    [DebuggerStepThrough]
    public static Memory<short> ToS16L(this ReadOnlyMemory<byte> mem) =>
        mem.Span.ToS16L();

    [DebuggerStepThrough]
    public static Memory<short> ToS16L(this Memory<byte> mem) =>
        mem.Span.ToS16L();

    [DebuggerStepThrough]
    public static Memory<short> ToS16B(this ReadOnlyMemory<byte> mem) =>
        mem.Span.ToS16B();

    [DebuggerStepThrough]
    public static Memory<short> ToS16B(this Memory<byte> mem) =>
        mem.Span.ToS16B();

    [DebuggerStepThrough]
    public static Memory<uint> ToU32L(this ReadOnlyMemory<byte> mem) =>
        mem.Span.ToU32L();

    [DebuggerStepThrough]
    public static Memory<uint> ToU32L(this Memory<byte> mem) =>
        mem.Span.ToU32L();

    [DebuggerStepThrough]
    public static Memory<uint> ToU32B(this ReadOnlyMemory<byte> mem) =>
        mem.Span.ToU32B();

    [DebuggerStepThrough]
    public static Memory<uint> ToU32B(this Memory<byte> mem) =>
        mem.Span.ToU32B();

    [DebuggerStepThrough]
    public static Memory<int> ToS32L(this ReadOnlyMemory<byte> mem) =>
        mem.Span.ToS32L();

    [DebuggerStepThrough]
    public static Memory<int> ToS32L(this Memory<byte> mem) =>
        mem.Span.ToS32L();

    [DebuggerStepThrough]
    public static Memory<int> ToS32B(this ReadOnlyMemory<byte> mem) =>
        mem.Span.ToS32B();

    [DebuggerStepThrough]
    public static Memory<int> ToS32B(this Memory<byte> mem) =>
        mem.Span.ToS32B();

    [DebuggerStepThrough]
    public static Memory<float> ToF32L(this ReadOnlyMemory<byte> mem) =>
        mem.Span.ToF32L();

    [DebuggerStepThrough]
    public static Memory<float> ToF32L(this Memory<byte> mem) =>
        mem.Span.ToF32L();

    [DebuggerStepThrough]
    public static Memory<float> ToF32B(this ReadOnlyMemory<byte> mem) =>
        mem.Span.ToF32B();

    [DebuggerStepThrough]
    public static Memory<float> ToF32B(this Memory<byte> mem) =>
        mem.Span.ToF32B();

    [DebuggerStepThrough]
    public static Memory<ulong> ToU64L(this ReadOnlyMemory<byte> mem) =>
        mem.Span.ToU64L();

    [DebuggerStepThrough]
    public static Memory<ulong> ToU64L(this Memory<byte> mem) =>
        mem.Span.ToU64L();

    [DebuggerStepThrough]
    public static Memory<ulong> ToU64B(this ReadOnlyMemory<byte> mem) =>
        mem.Span.ToU64B();

    [DebuggerStepThrough]
    public static Memory<ulong> ToU64B(this Memory<byte> mem) =>
        mem.Span.ToU64B();

    [DebuggerStepThrough]
    public static Memory<long> ToS64L(this ReadOnlyMemory<byte> mem) =>
        mem.Span.ToS64L();

    [DebuggerStepThrough]
    public static Memory<long> ToS64L(this Memory<byte> mem) =>
        mem.Span.ToS64L();

    [DebuggerStepThrough]
    public static Memory<long> ToS64B(this ReadOnlyMemory<byte> mem) =>
        mem.Span.ToS64B();

    [DebuggerStepThrough]
    public static Memory<long> ToS64B(this Memory<byte> mem) =>
        mem.Span.ToS64B();

    [DebuggerStepThrough]
    public static Memory<double> ToF64L(this ReadOnlyMemory<byte> mem) =>
        mem.Span.ToF64L();

    [DebuggerStepThrough]
    public static Memory<double> ToF64L(this Memory<byte> mem) =>
        mem.Span.ToF64L();

    [DebuggerStepThrough]
    public static Memory<double> ToF64B(this ReadOnlyMemory<byte> mem) =>
        mem.Span.ToF64B();

    [DebuggerStepThrough]
    public static Memory<double> ToF64B(this Memory<byte> mem) =>
        mem.Span.ToF64B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this ReadOnlyMemory<ushort> mem) =>
        mem.Span.ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this Memory<ushort> mem) =>
        mem.Span.ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this ReadOnlyMemory<short> mem) =>
        mem.Span.ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this Memory<short> mem) =>
        mem.Span.ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this ReadOnlyMemory<uint> mem) =>
        mem.Span.ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this Memory<uint> mem) =>
        mem.Span.ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this ReadOnlyMemory<int> mem) =>
        mem.Span.ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this Memory<int> mem) =>
        mem.Span.ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this ReadOnlyMemory<float> mem) =>
        mem.Span.ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this Memory<float> mem) =>
        mem.Span.ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this ReadOnlyMemory<ulong> mem) =>
        mem.Span.ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this Memory<ulong> mem) =>
        mem.Span.ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this ReadOnlyMemory<long> mem) =>
        mem.Span.ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this Memory<long> mem) =>
        mem.Span.ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this ReadOnlyMemory<double> mem) =>
        mem.Span.ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this Memory<double> mem) =>
        mem.Span.ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this ReadOnlyMemory<ushort> mem) =>
        mem.Span.ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this Memory<ushort> mem) =>
        mem.Span.ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this ReadOnlyMemory<short> mem) =>
        mem.Span.ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this Memory<short> mem) =>
        mem.Span.ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this ReadOnlyMemory<uint> mem) =>
        mem.Span.ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this Memory<uint> mem) =>
        mem.Span.ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this ReadOnlyMemory<int> mem) =>
        mem.Span.ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this Memory<int> mem) =>
        mem.Span.ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this ReadOnlyMemory<float> mem) =>
        mem.Span.ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this Memory<float> mem) =>
        mem.Span.ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this ReadOnlyMemory<ulong> mem) =>
        mem.Span.ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this Memory<ulong> mem) =>
        mem.Span.ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this ReadOnlyMemory<long> mem) =>
        mem.Span.ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this Memory<long> mem) =>
        mem.Span.ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this ReadOnlyMemory<double> mem) =>
        mem.Span.ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this Memory<double> mem) =>
        mem.Span.ToU8B();

    #endregion

    // ***************************************************

    #region Interpretations

    [DebuggerStepThrough]
    public static ReadOnlySpan<ushort> CastU16L(this ReadOnlyMemory<byte> mem) =>
        mem.Span.CastU16L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<ushort> CastU16L(this Memory<byte> mem) =>
        mem.Span.CastU16L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<ushort> CastU16B(this ReadOnlyMemory<byte> mem) =>
        mem.Span.CastU16B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<ushort> CastU16B(this Memory<byte> mem) =>
        mem.Span.CastU16B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<short> CastS16L(this ReadOnlyMemory<byte> mem) =>
        mem.Span.CastS16L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<short> CastS16L(this Memory<byte> mem) =>
        mem.Span.CastS16L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<short> CastS16B(this ReadOnlyMemory<byte> mem) =>
        mem.Span.CastS16B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<short> CastS16B(this Memory<byte> mem) =>
        mem.Span.CastS16B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<uint> CastU32L(this ReadOnlyMemory<byte> mem) =>
        mem.Span.CastU32L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<uint> CastU32L(this Memory<byte> mem) =>
        mem.Span.CastU32L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<uint> CastU32B(this ReadOnlyMemory<byte> mem) =>
        mem.Span.CastU32B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<uint> CastU32B(this Memory<byte> mem) =>
        mem.Span.CastU32B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<int> CastS32L(this ReadOnlyMemory<byte> mem) =>
        mem.Span.CastS32L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<int> CastS32L(this Memory<byte> mem) =>
        mem.Span.CastS32L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<int> CastS32B(this ReadOnlyMemory<byte> mem) =>
        mem.Span.CastS32B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<int> CastS32B(this Memory<byte> mem) =>
        mem.Span.CastS32B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<float> CastF32L(this ReadOnlyMemory<byte> mem) =>
        mem.Span.CastF32L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<float> CastF32L(this Memory<byte> mem) =>
        mem.Span.CastF32L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<float> CastF32B(this ReadOnlyMemory<byte> mem) =>
        mem.Span.CastF32B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<float> CastF32B(this Memory<byte> mem) =>
        mem.Span.CastF32B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<ulong> CastU64L(this ReadOnlyMemory<byte> mem) =>
        mem.Span.CastU64L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<ulong> CastU64L(this Memory<byte> mem) =>
        mem.Span.CastU64L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<ulong> CastU64B(this ReadOnlyMemory<byte> mem) =>
        mem.Span.CastU64B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<ulong> CastU64B(this Memory<byte> mem) =>
        mem.Span.CastU64B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<long> CastS64L(this ReadOnlyMemory<byte> mem) =>
        mem.Span.CastS64L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<long> CastS64L(this Memory<byte> mem) =>
        mem.Span.CastS64L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<long> CastS64B(this ReadOnlyMemory<byte> mem) =>
        mem.Span.CastS64B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<long> CastS64B(this Memory<byte> mem) =>
        mem.Span.CastS64B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<double> CastF64L(this ReadOnlyMemory<byte> mem) =>
        mem.Span.CastF64L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<double> CastF64L(this Memory<byte> mem) =>
        mem.Span.CastF64L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<double> CastF64B(this ReadOnlyMemory<byte> mem) =>
        mem.Span.CastF64B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<double> CastF64B(this Memory<byte> mem) =>
        mem.Span.CastF64B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this ReadOnlyMemory<ushort> mem) =>
        mem.Span.CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this Memory<ushort> mem) =>
        mem.Span.CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this ReadOnlyMemory<short> mem) =>
        mem.Span.CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this Memory<short> mem) =>
        mem.Span.CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this ReadOnlyMemory<uint> mem) =>
        mem.Span.CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this Memory<uint> mem) =>
        mem.Span.CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this ReadOnlyMemory<int> mem) =>
        mem.Span.CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this Memory<int> mem) =>
        mem.Span.CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this ReadOnlyMemory<float> mem) =>
        mem.Span.CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this Memory<float> mem) =>
        mem.Span.CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this ReadOnlyMemory<ulong> mem) =>
        mem.Span.CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this Memory<ulong> mem) =>
        mem.Span.CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this ReadOnlyMemory<long> mem) =>
        mem.Span.CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this Memory<long> mem) =>
        mem.Span.CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this ReadOnlyMemory<double> mem) =>
        mem.Span.CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this Memory<double> mem) =>
        mem.Span.CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this ReadOnlyMemory<ushort> mem) =>
        mem.Span.CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this Memory<ushort> mem) =>
        mem.Span.CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this ReadOnlyMemory<short> mem) =>
        mem.Span.CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this Memory<short> mem) =>
        mem.Span.CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this ReadOnlyMemory<uint> mem) =>
        mem.Span.CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this Memory<uint> mem) =>
        mem.Span.CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this ReadOnlyMemory<int> mem) =>
        mem.Span.CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this Memory<int> mem) =>
        mem.Span.CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this ReadOnlyMemory<float> mem) =>
        mem.Span.CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this Memory<float> mem) =>
        mem.Span.CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this ReadOnlyMemory<ulong> mem) =>
        mem.Span.CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this Memory<ulong> mem) =>
        mem.Span.CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this ReadOnlyMemory<long> mem) =>
        mem.Span.CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this Memory<long> mem) =>
        mem.Span.CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this ReadOnlyMemory<double> mem) =>
        mem.Span.CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this Memory<double> mem) =>
        mem.Span.CastU8B();

    #endregion

    // ***************************************************
}