using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace RhythmCodex.Utils.Cursors;

[PublicAPI]
public static class ByteMemoryExtensions
{
    // ***************************************************

    #region ReadOnlyMemory

    extension(ReadOnlyMemory<byte> memory)
    {
        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadU8(out byte val)
        {
            val = memory.Span[0];
            return memory[1..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadS8(out sbyte val)
        {
            val = MemoryMarshal.Cast<byte, sbyte>(memory.Span)[0];
            return memory[1..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadU16L(out ushort val)
        {
            val = ReadUInt16LittleEndian(memory.Span);
            return memory[2..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadS16L(out short val)
        {
            val = ReadInt16LittleEndian(memory.Span);
            return memory[2..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadU16B(out ushort val)
        {
            val = ReadUInt16BigEndian(memory.Span);
            return memory[2..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadS16B(out short val)
        {
            val = ReadInt16BigEndian(memory.Span);
            return memory[2..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadU32L(out uint val)
        {
            val = ReadUInt32LittleEndian(memory.Span);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadS32L(out int val)
        {
            val = ReadInt32LittleEndian(memory.Span);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadU32B(out uint val)
        {
            val = ReadUInt32BigEndian(memory.Span);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadS32B(out int val)
        {
            val = ReadInt32BigEndian(memory.Span);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadF32L(out float val)
        {
            val = ReadSingleLittleEndian(memory.Span);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadF32B(out float val)
        {
            val = ReadSingleBigEndian(memory.Span);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadU64L(out ulong val)
        {
            val = ReadUInt64LittleEndian(memory.Span);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadS64L(out long val)
        {
            val = ReadInt64LittleEndian(memory.Span);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadU64B(out ulong val)
        {
            val = ReadUInt64BigEndian(memory.Span);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadS64B(out long val)
        {
            val = ReadInt64BigEndian(memory.Span);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadF64L(out double val)
        {
            val = ReadDoubleLittleEndian(memory.Span);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> ReadF64B(out double val)
        {
            val = ReadDoubleBigEndian(memory.Span);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> Skip([NonNegativeValue] int count)
        {
            return memory[count..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> Extract([NonNegativeValue] int count,
            out ReadOnlyMemory<byte> val)
        {
            val = memory[..count];
            return memory[count..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> TryExtract(int count,
            out ReadOnlyMemory<byte> val)
        {
            val = memory[..Math.Min(memory.Length, count)];
            return memory[val.Length..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> Read([NonNegativeValue] int count,
            out byte[] val)
        {
            val = memory[..count].ToArray();
            return memory[count..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> Read(Span<byte> target,
            [NonNegativeValue] out int count)
        {
            count = target.Length;
            memory[..count].Span.CopyTo(target);
            return memory[count..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> Read(Memory<byte> target,
            [NonNegativeValue] out int count)
        {
            count = target.Length;
            memory[..count].CopyTo(target);
            return memory[count..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> TryRead([NonNegativeValue] int count,
            out byte[] val)
        {
            val = memory[..Math.Min(memory.Length, count)].ToArray();
            return memory[val.Length..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> TryRead(Span<byte> target,
            [NonNegativeValue] out int count)
        {
            count = target.Length;
            memory[..Math.Min(memory.Length, count)].Span.CopyTo(target);
            return memory[count..];
        }

        [DebuggerStepThrough]
        public ReadOnlyMemory<byte> TryRead(Memory<byte> target,
            [NonNegativeValue] out int count)
        {
            count = target.Length;
            memory[..Math.Min(memory.Length, count)].CopyTo(target);
            return memory[count..];
        }
    }

    #endregion

    // ***************************************************

    #region Memory

    extension(Memory<byte> memory)
    {
        [DebuggerStepThrough]
        public Memory<byte> ReadU8(out byte val)
        {
            val = memory.Span[0];
            return memory[1..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteU8(byte val)
        {
            memory.Span[0] = val;
            return memory[1..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadS8(out sbyte val)
        {
            val = MemoryMarshal.Cast<byte, sbyte>(memory.Span)[0];
            return memory[1..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteS8(sbyte val)
        {
            MemoryMarshal.Cast<byte, sbyte>(memory.Span)[0] = val;
            return memory[1..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadU16L(out ushort val)
        {
            val = ReadUInt16LittleEndian(memory.Span);
            return memory[2..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteU16L(ushort val)
        {
            WriteUInt16LittleEndian(memory.Span, val);
            return memory[2..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadS16L(out short val)
        {
            val = ReadInt16LittleEndian(memory.Span);
            return memory[2..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteS16L(short val)
        {
            WriteInt16LittleEndian(memory.Span, val);
            return memory[2..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadU16B(out ushort val)
        {
            val = ReadUInt16BigEndian(memory.Span);
            return memory[2..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteU16B(ushort val)
        {
            WriteUInt16BigEndian(memory.Span, val);
            return memory[2..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadS16B(out short val)
        {
            val = ReadInt16BigEndian(memory.Span);
            return memory[2..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteS16B(short val)
        {
            WriteInt16BigEndian(memory.Span, val);
            return memory[2..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadU32L(out uint val)
        {
            val = ReadUInt32LittleEndian(memory.Span);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteU32L(uint val)
        {
            WriteUInt32LittleEndian(memory.Span, val);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadS32L(out int val)
        {
            val = ReadInt32LittleEndian(memory.Span);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteS32L(int val)
        {
            WriteInt32LittleEndian(memory.Span, val);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadU32B(out uint val)
        {
            val = ReadUInt32BigEndian(memory.Span);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteU32B(uint val)
        {
            WriteUInt32BigEndian(memory.Span, val);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadS32B(out int val)
        {
            val = ReadInt32BigEndian(memory.Span);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteS32B(int val)
        {
            WriteInt32BigEndian(memory.Span, val);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadF32L(out float val)
        {
            val = ReadSingleLittleEndian(memory.Span);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteF32L(float val)
        {
            WriteSingleLittleEndian(memory.Span, val);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadF32B(out float val)
        {
            val = ReadSingleBigEndian(memory.Span);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteF32B(float val)
        {
            WriteSingleBigEndian(memory.Span, val);
            return memory[4..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadU64L(out ulong val)
        {
            val = ReadUInt64LittleEndian(memory.Span);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteU64L(ulong val)
        {
            WriteUInt64LittleEndian(memory.Span, val);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadS64L(out long val)
        {
            val = ReadInt64LittleEndian(memory.Span);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteS64L(long val)
        {
            WriteInt64LittleEndian(memory.Span, val);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadU64B(out ulong val)
        {
            val = ReadUInt64BigEndian(memory.Span);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteU64B(ulong val)
        {
            WriteUInt64BigEndian(memory.Span, val);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadS64B(out long val)
        {
            val = ReadInt64BigEndian(memory.Span);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteS64B(long val)
        {
            WriteInt64BigEndian(memory.Span, val);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadF64L(out double val)
        {
            val = ReadDoubleLittleEndian(memory.Span);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteF64L(double val)
        {
            WriteDoubleLittleEndian(memory.Span, val);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public Memory<byte> ReadF64B(out double val)
        {
            val = ReadDoubleBigEndian(memory.Span);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public Memory<byte> WriteF64B(double val)
        {
            WriteDoubleBigEndian(memory.Span, val);
            return memory[8..];
        }

        [DebuggerStepThrough]
        public Memory<byte> Skip([NonNegativeValue] int count)
        {
            return memory[count..];
        }

        [DebuggerStepThrough]
        public Memory<byte> Extract([NonNegativeValue] int count,
            out Memory<byte> val)
        {
            val = memory[..count];
            return memory[count..];
        }

        [DebuggerStepThrough]
        public Memory<byte> TryExtract(int count,
            out Memory<byte> val)
        {
            val = memory[..Math.Min(memory.Length, count)];
            return memory[val.Length..];
        }

        [DebuggerStepThrough]
        public Memory<byte> Read([NonNegativeValue] int count,
            out byte[] val)
        {
            val = memory[..count].ToArray();
            return memory[count..];
        }

        [DebuggerStepThrough]
        public Memory<byte> Read(Span<byte> target,
            [NonNegativeValue] out int count)
        {
            count = target.Length;
            memory[..count].Span.CopyTo(target);
            return memory[count..];
        }

        [DebuggerStepThrough]
        public Memory<byte> Read(Memory<byte> target,
            [NonNegativeValue] out int count)
        {
            count = target.Length;
            memory[..count].CopyTo(target);
            return memory[count..];
        }

        [DebuggerStepThrough]
        public Memory<byte> TryRead([NonNegativeValue] int count,
            out byte[] val)
        {
            val = memory[..Math.Min(memory.Length, count)].ToArray();
            return memory[val.Length..];
        }

        [DebuggerStepThrough]
        public Memory<byte> TryRead(Span<byte> target,
            [NonNegativeValue] out int count)
        {
            count = target.Length;
            memory[..Math.Min(memory.Length, count)].Span.CopyTo(target);
            return memory[count..];
        }

        [DebuggerStepThrough]
        public Memory<byte> TryRead(Memory<byte> target,
            [NonNegativeValue] out int count)
        {
            count = target.Length;
            memory[..Math.Min(memory.Length, count)].CopyTo(target);
            return memory[count..];
        }

        [DebuggerStepThrough]
        public Memory<byte> Write(ReadOnlySpan<byte> val,
            out int count)
        {
            count = val.Length;
            val.CopyTo(memory.Span);
            return memory[count..];
        }

        [DebuggerStepThrough]
        public Memory<byte> Write(ReadOnlyMemory<byte> val,
            out int count)
        {
            count = val.Length;
            val.CopyTo(memory);
            return memory[count..];
        }

        [DebuggerStepThrough]
        public Memory<byte> TryWrite(ReadOnlySpan<byte> val,
            out int count)
        {
            count = Math.Min(memory.Length, val.Length);
            val[..count].CopyTo(memory.Span);
            return memory[count..];
        }

        [DebuggerStepThrough]
        public Memory<byte> TryWrite(ReadOnlyMemory<byte> val,
            out int count)
        {
            count = Math.Min(memory.Length, val.Length);
            val[..count].CopyTo(memory);
            return memory[count..];
        }
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