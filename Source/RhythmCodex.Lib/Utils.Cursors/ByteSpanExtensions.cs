using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace RhythmCodex.Utils.Cursors;

[PublicAPI]
public static class ByteSpanExtensions
{
    // ***************************************************

    #region ReadOnlySpan

    extension(ReadOnlySpan<byte> span)
    {
        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadU8(out byte val)
        {
            val = span[0];
            return span[1..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadS8(out sbyte val)
        {
            val = MemoryMarshal.Cast<byte, sbyte>(span)[0];
            return span[1..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadU16L(out ushort val)
        {
            val = ReadUInt16LittleEndian(span);
            return span[2..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadS16L(out short val)
        {
            val = ReadInt16LittleEndian(span);
            return span[2..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadU16B(out ushort val)
        {
            val = ReadUInt16BigEndian(span);
            return span[2..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadS16B(out short val)
        {
            val = ReadInt16BigEndian(span);
            return span[2..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadU32L(out uint val)
        {
            val = ReadUInt32LittleEndian(span);
            return span[4..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadS32L(out int val)
        {
            val = ReadInt32LittleEndian(span);
            return span[4..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadU32B(out uint val)
        {
            val = ReadUInt32BigEndian(span);
            return span[4..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadS32B(out int val)
        {
            val = ReadInt32BigEndian(span);
            return span[4..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadF32L(out float val)
        {
            val = ReadSingleLittleEndian(span);
            return span[4..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadF32B(out float val)
        {
            val = ReadSingleBigEndian(span);
            return span[4..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadU64L(out ulong val)
        {
            val = ReadUInt64LittleEndian(span);
            return span[8..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadS64L(out long val)
        {
            val = ReadInt64LittleEndian(span);
            return span[8..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadU64B(out ulong val)
        {
            val = ReadUInt64BigEndian(span);
            return span[8..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadS64B(out long val)
        {
            val = ReadInt64BigEndian(span);
            return span[8..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadF64L(out double val)
        {
            val = ReadDoubleLittleEndian(span);
            return span[8..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> ReadF64B(out double val)
        {
            val = ReadDoubleBigEndian(span);
            return span[8..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> Skip([NonNegativeValue] int count)
        {
            return span[count..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> Extract([NonNegativeValue] int count,
            out ReadOnlySpan<byte> val)
        {
            val = span[..count];
            return span[count..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> TryExtract(int count,
            out ReadOnlySpan<byte> val)
        {
            val = span[..Math.Min(span.Length, count)];
            return span[val.Length..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> Read([NonNegativeValue] int count,
            out byte[] val)
        {
            val = span[..count].ToArray();
            return span[count..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> Read(Memory<byte> target,
            [NonNegativeValue] out int count)
        {
            count = target.Length;
            span[..count].CopyTo(target.Span);
            return span[count..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> Read(Span<byte> target,
            [NonNegativeValue] out int count)
        {
            count = target.Length;
            span[..count].CopyTo(target);
            return span[count..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> TryRead([NonNegativeValue] int count,
            out byte[] val)
        {
            val = span[..Math.Min(span.Length, count)].ToArray();
            return span[val.Length..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> TryRead(Memory<byte> target,
            [NonNegativeValue] out int count)
        {
            count = target.Length;
            span[..Math.Min(span.Length, count)].CopyTo(target.Span);
            return span[count..];
        }

        [DebuggerStepThrough]
        public ReadOnlySpan<byte> TryRead(Span<byte> target,
            [NonNegativeValue] out int count)
        {
            count = target.Length;
            span[..Math.Min(span.Length, count)].CopyTo(target);
            return span[count..];
        }

        [DebuggerStepThrough]
        public byte AsU8() => span[0];

        [DebuggerStepThrough]
        public sbyte AsS8() => unchecked((sbyte)span[0]);

        [DebuggerStepThrough]
        public ushort AsU16L() => ReadUInt16LittleEndian(span);

        [DebuggerStepThrough]
        public short AsS16L() => ReadInt16LittleEndian(span);

        [DebuggerStepThrough]
        public ushort AsU16B() => ReadUInt16BigEndian(span);

        [DebuggerStepThrough]
        public short AsS16B() => ReadInt16BigEndian(span);

        [DebuggerStepThrough]
        public uint AsU24L() => ((uint)ReadUInt16LittleEndian(span[1..]) << 8) | span[0];

        [DebuggerStepThrough]
        public int AsS24L() => (ReadUInt16LittleEndian(span[1..]) << 8) | span[0];

        [DebuggerStepThrough]
        public uint AsU24B() => ((uint)ReadUInt16BigEndian(span) << 8) | span[2];

        [DebuggerStepThrough]
        public int AsS24B() => (ReadUInt16BigEndian(span) << 8) | span[2];

        [DebuggerStepThrough]
        public uint AsU32L() => ReadUInt32LittleEndian(span);

        [DebuggerStepThrough]
        public int AsS32L() => ReadInt32LittleEndian(span);

        [DebuggerStepThrough]
        public uint AsU32B() => ReadUInt32BigEndian(span);

        [DebuggerStepThrough]
        public int AsS32B() => ReadInt32BigEndian(span);

        [DebuggerStepThrough]
        public float AsF32L() => ReadSingleLittleEndian(span);

        [DebuggerStepThrough]
        public float AsF32B() => ReadSingleBigEndian(span);

        [DebuggerStepThrough]
        public double AsF64L() => ReadDoubleLittleEndian(span);

        [DebuggerStepThrough]
        public double AsF64B() => ReadDoubleBigEndian(span);
    }

    #endregion

    // ***************************************************

    #region Span

    extension(Span<byte> span)
    {
        [DebuggerStepThrough]
        public Span<byte> ReadU8(out byte val)
        {
            val = span[0];
            return span[1..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteU8(byte val)
        {
            span[0] = val;
            return span[1..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadS8(out sbyte val)
        {
            val = MemoryMarshal.Cast<byte, sbyte>(span)[0];
            return span[1..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteS8(sbyte val)
        {
            MemoryMarshal.Cast<byte, sbyte>(span)[0] = val;
            return span[1..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadU16L(out ushort val)
        {
            val = ReadUInt16LittleEndian(span);
            return span[2..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteU16L(ushort val)
        {
            WriteUInt16LittleEndian(span, val);
            return span[2..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadS16L(out short val)
        {
            val = ReadInt16LittleEndian(span);
            return span[2..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteS16L(short val)
        {
            WriteInt16LittleEndian(span, val);
            return span[2..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadU16B(out ushort val)
        {
            val = ReadUInt16BigEndian(span);
            return span[2..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteU16B(ushort val)
        {
            WriteUInt16BigEndian(span, val);
            return span[2..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadS16B(out short val)
        {
            val = ReadInt16BigEndian(span);
            return span[2..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteS16B(short val)
        {
            WriteInt16BigEndian(span, val);
            return span[2..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadU32L(out uint val)
        {
            val = ReadUInt32LittleEndian(span);
            return span[4..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteU32L(uint val)
        {
            WriteUInt32LittleEndian(span, val);
            return span[4..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadS32L(out int val)
        {
            val = ReadInt32LittleEndian(span);
            return span[4..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteS32L(int val)
        {
            WriteInt32LittleEndian(span, val);
            return span[4..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadU32B(out uint val)
        {
            val = ReadUInt32BigEndian(span);
            return span[4..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteU32B(uint val)
        {
            WriteUInt32BigEndian(span, val);
            return span[4..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadS32B(out int val)
        {
            val = ReadInt32BigEndian(span);
            return span[4..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteS32B(int val)
        {
            WriteInt32BigEndian(span, val);
            return span[4..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadF32L(out float val)
        {
            val = ReadSingleLittleEndian(span);
            return span[4..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteF32L(float val)
        {
            WriteSingleLittleEndian(span, val);
            return span[4..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadF32B(out float val)
        {
            val = ReadSingleBigEndian(span);
            return span[4..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteF32B(float val)
        {
            WriteSingleBigEndian(span, val);
            return span[4..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadU64L(out ulong val)
        {
            val = ReadUInt64LittleEndian(span);
            return span[8..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteU64L(ulong val)
        {
            WriteUInt64LittleEndian(span, val);
            return span[8..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadS64L(out long val)
        {
            val = ReadInt64LittleEndian(span);
            return span[8..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteS64L(long val)
        {
            WriteInt64LittleEndian(span, val);
            return span[8..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadU64B(out ulong val)
        {
            val = ReadUInt64BigEndian(span);
            return span[8..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteU64B(ulong val)
        {
            WriteUInt64BigEndian(span, val);
            return span[8..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadS64B(out long val)
        {
            val = ReadInt64BigEndian(span);
            return span[8..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteS64B(long val)
        {
            WriteInt64BigEndian(span, val);
            return span[8..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadF64L(out double val)
        {
            val = ReadDoubleLittleEndian(span);
            return span[8..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteF64L(double val)
        {
            WriteDoubleLittleEndian(span, val);
            return span[8..];
        }

        [DebuggerStepThrough]
        public Span<byte> ReadF64B(out double val)
        {
            val = ReadDoubleBigEndian(span);
            return span[8..];
        }

        [DebuggerStepThrough]
        public Span<byte> WriteF64B(double val)
        {
            WriteDoubleBigEndian(span, val);
            return span[8..];
        }

        [DebuggerStepThrough]
        public Span<byte> Skip([NonNegativeValue] int count)
        {
            return span[count..];
        }

        [DebuggerStepThrough]
        public Span<byte> Extract([NonNegativeValue] int count,
            out Span<byte> val)
        {
            val = span[..count];
            return span[count..];
        }

        [DebuggerStepThrough]
        public Span<byte> TryExtract(int count,
            out Span<byte> val)
        {
            val = span[..Math.Min(span.Length, count)];
            return span[val.Length..];
        }

        [DebuggerStepThrough]
        public Span<byte> Read([NonNegativeValue] int count,
            out byte[] val)
        {
            val = span[..count].ToArray();
            return span[count..];
        }

        [DebuggerStepThrough]
        public Span<byte> Read(Span<byte> target,
            [NonNegativeValue] out int count)
        {
            count = target.Length;
            span[..count].CopyTo(target);
            return span[count..];
        }

        [DebuggerStepThrough]
        public Span<byte> Read(Memory<byte> target,
            [NonNegativeValue] out int count)
        {
            count = target.Length;
            span[..count].CopyTo(target.Span);
            return span[count..];
        }

        [DebuggerStepThrough]
        public Span<byte> TryRead([NonNegativeValue] int count,
            out byte[] val)
        {
            val = span[..Math.Min(span.Length, count)].ToArray();
            return span[val.Length..];
        }

        [DebuggerStepThrough]
        public Span<byte> TryRead(Span<byte> target,
            [NonNegativeValue] out int count)
        {
            count = Math.Min(span.Length, target.Length);
            span[..count].CopyTo(target);
            return span[count..];
        }

        [DebuggerStepThrough]
        public Span<byte> TryRead(Memory<byte> target,
            [NonNegativeValue] out int count)
        {
            count = target.Length;
            span[..Math.Min(span.Length, count)].CopyTo(target.Span);
            return span[count..];
        }

        [DebuggerStepThrough]
        public Span<byte> Write(ReadOnlySpan<byte> val,
            out int count)
        {
            count = val.Length;
            val.CopyTo(span);
            return span[count..];
        }

        [DebuggerStepThrough]
        public Span<byte> Write(ReadOnlyMemory<byte> val,
            out int count)
        {
            count = val.Length;
            val.Span.CopyTo(span);
            return span[count..];
        }

        [DebuggerStepThrough]
        public Span<byte> TryWrite(ReadOnlyMemory<byte> val,
            out int count)
        {
            count = Math.Min(span.Length, val.Length);
            val[..count].Span.CopyTo(span);
            return span[count..];
        }

        [DebuggerStepThrough]
        public Span<byte> TryWrite(ReadOnlySpan<byte> val,
            out int count)
        {
            count = Math.Min(span.Length, val.Length);
            val[..count].CopyTo(span);
            return span[count..];
        }

        [DebuggerStepThrough]
        public byte AsU8() => span[0];

        [DebuggerStepThrough]
        public sbyte AsS8() => unchecked((sbyte)span[0]);

        [DebuggerStepThrough]
        public ushort AsU16L() => ReadUInt16LittleEndian(span);

        [DebuggerStepThrough]
        public short AsS16L() => ReadInt16LittleEndian(span);

        [DebuggerStepThrough]
        public ushort AsU16B() => ReadUInt16BigEndian(span);

        [DebuggerStepThrough]
        public short AsS16B() => ReadInt16BigEndian(span);

        [DebuggerStepThrough]
        public uint AsU24L() => ((uint)ReadUInt16LittleEndian(span[1..]) << 8) | span[0];

        [DebuggerStepThrough]
        public int AsS24L() => (ReadUInt16LittleEndian(span[1..]) << 8) | span[0];

        [DebuggerStepThrough]
        public uint AsU24B() => ((uint)ReadUInt16BigEndian(span) << 8) | span[2];

        [DebuggerStepThrough]
        public int AsS24B() => (ReadUInt16BigEndian(span) << 8) | span[2];

        [DebuggerStepThrough]
        public uint AsU32L() => ReadUInt32LittleEndian(span);

        [DebuggerStepThrough]
        public int AsS32L() => ReadInt32LittleEndian(span);

        [DebuggerStepThrough]
        public uint AsU32B() => ReadUInt32BigEndian(span);

        [DebuggerStepThrough]
        public int AsS32B() => ReadInt32BigEndian(span);

        [DebuggerStepThrough]
        public float AsF32L() => ReadSingleLittleEndian(span);

        [DebuggerStepThrough]
        public float AsF32B() => ReadSingleBigEndian(span);

        [DebuggerStepThrough]
        public double AsF64L() => ReadDoubleLittleEndian(span);

        [DebuggerStepThrough]
        public double AsF64B() => ReadDoubleBigEndian(span);
    }

    #endregion

    // ***************************************************

    #region Conversions

    [DebuggerStepThrough]
    public static Memory<ushort> ToU16L(
        this ReadOnlySpan<byte> span)
    {
        var output = new ushort[span.Length / 2];
        for (int i = 0, j = 0; i < output.Length; i++, j += 2)
            output[i] = ReadUInt16LittleEndian(span[j..]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<ushort> ToU16L(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).ToU16L();

    [DebuggerStepThrough]
    public static Memory<ushort> ToU16B(
        this ReadOnlySpan<byte> span)
    {
        var output = new ushort[span.Length / 2];
        for (int i = 0, j = 0; i < output.Length; i++, j += 2)
            output[i] = ReadUInt16BigEndian(span[j..]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<ushort> ToU16B(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).ToU16B();

    [DebuggerStepThrough]
    public static Memory<short> ToS16L(
        this ReadOnlySpan<byte> span)
    {
        var output = new short[span.Length / 2];
        for (int i = 0, j = 0; i < output.Length; i++, j += 2)
            output[i] = ReadInt16LittleEndian(span[j..]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<short> ToS16L(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).ToS16L();

    [DebuggerStepThrough]
    public static Memory<short> ToS16B(
        this ReadOnlySpan<byte> span)
    {
        var output = new short[span.Length / 2];
        for (int i = 0, j = 0; i < output.Length; i++, j += 2)
            output[i] = ReadInt16BigEndian(span[j..]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<short> ToS16B(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).ToS16B();

    [DebuggerStepThrough]
    public static Memory<uint> ToU32L(
        this ReadOnlySpan<byte> span)
    {
        var output = new uint[span.Length / 4];
        for (int i = 0, j = 0; i < output.Length; i++, j += 4)
            output[i] = ReadUInt32LittleEndian(span[j..]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<uint> ToU32L(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).ToU32L();

    [DebuggerStepThrough]
    public static Memory<uint> ToU32B(
        this ReadOnlySpan<byte> span)
    {
        var output = new uint[span.Length / 4];
        for (int i = 0, j = 0; i < output.Length; i++, j += 4)
            output[i] = ReadUInt32BigEndian(span[j..]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<uint> ToU32B(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).ToU32B();

    [DebuggerStepThrough]
    public static Memory<int> ToS32L(
        this ReadOnlySpan<byte> span)
    {
        var output = new int[span.Length / 4];
        for (int i = 0, j = 0; i < output.Length; i++, j += 4)
            output[i] = ReadInt32LittleEndian(span[j..]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<int> ToS32L(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).ToS32L();

    [DebuggerStepThrough]
    public static Memory<int> ToS32B(
        this ReadOnlySpan<byte> span)
    {
        var output = new int[span.Length / 4];
        for (int i = 0, j = 0; i < output.Length; i++, j += 4)
            output[i] = ReadInt32BigEndian(span[j..]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<int> ToS32B(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).ToS32B();

    [DebuggerStepThrough]
    public static Memory<float> ToF32L(
        this ReadOnlySpan<byte> span)
    {
        var output = new float[span.Length / 4];
        for (int i = 0, j = 0; i < output.Length; i++, j += 4)
            output[i] = ReadSingleLittleEndian(span[j..]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<float> ToF32L(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).ToF32L();

    [DebuggerStepThrough]
    public static Memory<float> ToF32B(
        this ReadOnlySpan<byte> span)
    {
        var output = new float[span.Length / 4];
        for (int i = 0, j = 0; i < output.Length; i++, j += 4)
            output[i] = ReadSingleBigEndian(span[j..]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<float> ToF32B(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).ToF32B();

    [DebuggerStepThrough]
    public static Memory<ulong> ToU64L(
        this ReadOnlySpan<byte> span)
    {
        var output = new ulong[span.Length / 8];
        for (int i = 0, j = 0; i < output.Length; i++, j += 8)
            output[i] = ReadUInt64LittleEndian(span[j..]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<ulong> ToU64L(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).ToU64L();

    [DebuggerStepThrough]
    public static Memory<ulong> ToU64B(
        this ReadOnlySpan<byte> span)
    {
        var output = new ulong[span.Length / 8];
        for (int i = 0, j = 0; i < output.Length; i++, j += 8)
            output[i] = ReadUInt64BigEndian(span[j..]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<ulong> ToU64B(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).ToU64B();

    [DebuggerStepThrough]
    public static Memory<long> ToS64L(
        this ReadOnlySpan<byte> span)
    {
        var output = new long[span.Length / 8];
        for (int i = 0, j = 0; i < output.Length; i++, j += 8)
            output[i] = ReadInt64LittleEndian(span[j..]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<long> ToS64L(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).ToS64L();

    [DebuggerStepThrough]
    public static Memory<long> ToS64B(
        this ReadOnlySpan<byte> span)
    {
        var output = new long[span.Length / 8];
        for (int i = 0, j = 0; i < output.Length; i++, j += 8)
            output[i] = ReadInt64BigEndian(span[j..]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<long> ToS64B(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).ToS64B();

    [DebuggerStepThrough]
    public static Memory<double> ToF64L(
        this ReadOnlySpan<byte> span)
    {
        var output = new double[span.Length / 8];
        for (int i = 0, j = 0; i < output.Length; i++, j += 8)
            output[i] = ReadDoubleLittleEndian(span[j..]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<double> ToF64L(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).ToF64L();

    [DebuggerStepThrough]
    public static Memory<double> ToF64B(
        this ReadOnlySpan<byte> span)
    {
        var output = new double[span.Length / 8];
        for (int i = 0, j = 0; i < output.Length; i++, j += 8)
            output[i] = ReadDoubleBigEndian(span[j..]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<double> ToF64B(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).ToF64B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(
        this ReadOnlySpan<ushort> span)
    {
        var output = new byte[span.Length * 2];
        for (int i = 0, j = 0; i < output.Length; i += 2, j++)
            WriteUInt16LittleEndian(output.AsSpan(i), span[j]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(
        this Span<ushort> span) =>
        ((ReadOnlySpan<ushort>)span).ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(
        this ReadOnlySpan<short> span)
    {
        var output = new byte[span.Length * 2];
        for (int i = 0, j = 0; i < output.Length; i += 2, j++)
            WriteInt16LittleEndian(output.AsSpan(i), span[j]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(
        this Span<short> span) =>
        ((ReadOnlySpan<short>)span).ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(
        this ReadOnlySpan<uint> span)
    {
        var output = new byte[span.Length * 4];
        for (int i = 0, j = 0; i < output.Length; i += 4, j++)
            WriteUInt32LittleEndian(output.AsSpan(i), span[j]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(
        this Span<uint> span) =>
        ((ReadOnlySpan<uint>)span).ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(
        this ReadOnlySpan<int> span)
    {
        var output = new byte[span.Length * 4];
        for (int i = 0, j = 0; i < output.Length; i += 4, j++)
            WriteInt32LittleEndian(output.AsSpan(i), span[j]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(
        this Span<int> span) =>
        ((ReadOnlySpan<int>)span).ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(
        this ReadOnlySpan<float> span)
    {
        var output = new byte[span.Length * 4];
        for (int i = 0, j = 0; i < output.Length; i += 4, j++)
            WriteSingleLittleEndian(output.AsSpan(i), span[j]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(
        this Span<float> span) =>
        ((ReadOnlySpan<float>)span).ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(
        this ReadOnlySpan<ulong> span)
    {
        var output = new byte[span.Length * 8];
        for (int i = 0, j = 0; i < output.Length; i += 8, j++)
            WriteUInt64LittleEndian(output.AsSpan(i), span[j]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(
        this Span<ulong> span) =>
        ((ReadOnlySpan<ulong>)span).ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(
        this ReadOnlySpan<long> span)
    {
        var output = new byte[span.Length * 8];
        for (int i = 0, j = 0; i < output.Length; i += 8, j++)
            WriteInt64LittleEndian(output.AsSpan(i), span[j]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(
        this Span<long> span) =>
        ((ReadOnlySpan<long>)span).ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(
        this ReadOnlySpan<double> span)
    {
        var output = new byte[span.Length * 8];
        for (int i = 0, j = 0; i < output.Length; i += 8, j++)
            WriteDoubleLittleEndian(output.AsSpan(i), span[j]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(
        this Span<double> span) =>
        ((ReadOnlySpan<double>)span).ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(
        this ReadOnlySpan<ushort> span)
    {
        var output = new byte[span.Length * 2];
        for (int i = 0, j = 0; i < output.Length; i += 2, j++)
            WriteUInt16BigEndian(output.AsSpan(i), span[j]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(
        this Span<ushort> span) =>
        ((ReadOnlySpan<ushort>)span).ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(
        this ReadOnlySpan<short> span)
    {
        var output = new byte[span.Length * 2];
        for (int i = 0, j = 0; i < output.Length; i += 2, j++)
            WriteInt16BigEndian(output.AsSpan(i), span[j]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(
        this Span<short> span) =>
        ((ReadOnlySpan<short>)span).ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(
        this ReadOnlySpan<uint> span)
    {
        var output = new byte[span.Length * 4];
        for (int i = 0, j = 0; i < output.Length; i += 4, j++)
            WriteUInt32BigEndian(output.AsSpan(i), span[j]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(
        this Span<uint> span) =>
        ((ReadOnlySpan<uint>)span).ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(
        this ReadOnlySpan<int> span)
    {
        var output = new byte[span.Length * 4];
        for (int i = 0, j = 0; i < output.Length; i += 4, j++)
            WriteInt32BigEndian(output.AsSpan(i), span[j]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(
        this Span<int> span) =>
        ((ReadOnlySpan<int>)span).ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(
        this ReadOnlySpan<float> span)
    {
        var output = new byte[span.Length * 4];
        for (int i = 0, j = 0; i < output.Length; i += 4, j++)
            WriteSingleBigEndian(output.AsSpan(i), span[j]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(
        this Span<float> span) =>
        ((ReadOnlySpan<float>)span).ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(
        this ReadOnlySpan<ulong> span)
    {
        var output = new byte[span.Length * 8];
        for (int i = 0, j = 0; i < output.Length; i += 8, j++)
            WriteUInt64BigEndian(output.AsSpan(i), span[j]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(
        this Span<ulong> span) =>
        ((ReadOnlySpan<ulong>)span).ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(
        this ReadOnlySpan<long> span)
    {
        var output = new byte[span.Length * 8];
        for (int i = 0, j = 0; i < output.Length; i += 8, j++)
            WriteInt64BigEndian(output.AsSpan(i), span[j]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(
        this Span<long> span) =>
        ((ReadOnlySpan<long>)span).ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(
        this ReadOnlySpan<double> span)
    {
        var output = new byte[span.Length * 8];
        for (int i = 0, j = 0; i < output.Length; i += 8, j++)
            WriteDoubleBigEndian(output.AsSpan(i), span[j]);
        return output;
    }

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(
        this Span<double> span) =>
        ((ReadOnlySpan<double>)span).ToU8B();

    #endregion

    // ***************************************************

    #region Interpretations

    [DebuggerStepThrough]
    public static ReadOnlySpan<ushort> CastU16L(
        this ReadOnlySpan<byte> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, ushort>(span)
            : span.ToU16L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<ushort> CastU16L(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).CastU16L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<ushort> CastU16B(
        this ReadOnlySpan<byte> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, ushort>(span)
            : span.ToU16B().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<ushort> CastU16B(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).CastU16B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<short> CastS16L(
        this ReadOnlySpan<byte> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, short>(span)
            : span.ToS16L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<short> CastS16L(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).CastS16L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<short> CastS16B(
        this ReadOnlySpan<byte> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, short>(span)
            : span.ToS16B().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<short> CastS16B(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).CastS16B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<uint> CastU32L(
        this ReadOnlySpan<byte> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, uint>(span)
            : span.ToU32L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<uint> CastU32L(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).CastU32L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<uint> CastU32B(
        this ReadOnlySpan<byte> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, uint>(span)
            : span.ToU32B().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<uint> CastU32B(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).CastU32B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<int> CastS32L(
        this ReadOnlySpan<byte> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, int>(span)
            : span.ToS32L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<int> CastS32L(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).CastS32L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<int> CastS32B(
        this ReadOnlySpan<byte> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, int>(span)
            : span.ToS32B().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<int> CastS32B(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).CastS32B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<float> CastF32L(
        this ReadOnlySpan<byte> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, float>(span)
            : span.ToF32L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<float> CastF32L(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).CastF32L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<float> CastF32B(
        this ReadOnlySpan<byte> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, float>(span)
            : span.ToF32L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<float> CastF32B(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).CastF32B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<ulong> CastU64L(
        this ReadOnlySpan<byte> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, ulong>(span)
            : span.ToU64L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<ulong> CastU64L(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).CastU64L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<ulong> CastU64B(
        this ReadOnlySpan<byte> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, ulong>(span)
            : span.ToU64B().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<ulong> CastU64B(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).CastU64B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<long> CastS64L(
        this ReadOnlySpan<byte> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, long>(span)
            : span.ToS64L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<long> CastS64L(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).CastS64L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<long> CastS64B(
        this ReadOnlySpan<byte> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, long>(span)
            : span.ToS64B().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<long> CastS64B(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).CastS64B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<double> CastF64L(
        this ReadOnlySpan<byte> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, double>(span)
            : span.ToF64L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<double> CastF64L(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).CastF64L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<double> CastF64B(
        this ReadOnlySpan<byte> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, double>(span)
            : span.ToF64B().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<double> CastF64B(
        this Span<byte> span) =>
        ((ReadOnlySpan<byte>)span).CastF64B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this ReadOnlySpan<ushort> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<ushort, byte>(span)
            : span.ToU8L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this Span<ushort> span) =>
        ((ReadOnlySpan<ushort>)span).CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this ReadOnlySpan<short> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<short, byte>(span)
            : span.ToU8L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this Span<short> span) =>
        ((ReadOnlySpan<short>)span).CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this ReadOnlySpan<uint> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<uint, byte>(span)
            : span.ToU8L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this Span<uint> span) =>
        ((ReadOnlySpan<uint>)span).CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this ReadOnlySpan<int> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<int, byte>(span)
            : span.ToU8L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this Span<int> span) =>
        ((ReadOnlySpan<int>)span).CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this ReadOnlySpan<float> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<float, byte>(span)
            : span.ToU8L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this Span<float> span) =>
        ((ReadOnlySpan<float>)span).CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this ReadOnlySpan<ulong> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<ulong, byte>(span)
            : span.ToU8L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this Span<ulong> span) =>
        ((ReadOnlySpan<ulong>)span).CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this ReadOnlySpan<long> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<long, byte>(span)
            : span.ToU8L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this Span<long> span) =>
        ((ReadOnlySpan<long>)span).CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this ReadOnlySpan<double> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<double, byte>(span)
            : span.ToU8L().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this Span<double> span) =>
        ((ReadOnlySpan<double>)span).CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this ReadOnlySpan<ushort> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<ushort, byte>(span)
            : span.ToU8B().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this Span<ushort> span) =>
        ((ReadOnlySpan<ushort>)span).CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this ReadOnlySpan<short> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<short, byte>(span)
            : span.ToU8B().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this Span<short> span) =>
        ((ReadOnlySpan<short>)span).CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this ReadOnlySpan<uint> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<uint, byte>(span)
            : span.ToU8B().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this Span<uint> span) =>
        ((ReadOnlySpan<uint>)span).CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this ReadOnlySpan<int> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<int, byte>(span)
            : span.ToU8B().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this Span<int> span) =>
        ((ReadOnlySpan<int>)span).CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this ReadOnlySpan<float> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<float, byte>(span)
            : span.ToU8B().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this Span<float> span) =>
        ((ReadOnlySpan<float>)span).CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this ReadOnlySpan<ulong> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<ulong, byte>(span)
            : span.ToU8B().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this Span<ulong> span) =>
        ((ReadOnlySpan<ulong>)span).CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this ReadOnlySpan<long> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<long, byte>(span)
            : span.ToU8B().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this Span<long> span) =>
        ((ReadOnlySpan<long>)span).CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this ReadOnlySpan<double> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<double, byte>(span)
            : span.ToU8B().Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this Span<double> span) =>
        ((ReadOnlySpan<double>)span).CastU8B();

    #endregion

    // ***************************************************
}