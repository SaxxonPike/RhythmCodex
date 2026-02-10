using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using JetBrains.Annotations;

namespace RhythmCodex.Utils.Cursors;

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
        val = ReadUInt16LittleEndian(span);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadS16L(
        this ReadOnlySpan<byte> span,
        out short val)
    {
        val = ReadInt16LittleEndian(span);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadU16B(
        this ReadOnlySpan<byte> span,
        out ushort val)
    {
        val = ReadUInt16BigEndian(span);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadS16B(
        this ReadOnlySpan<byte> span,
        out short val)
    {
        val = ReadInt16BigEndian(span);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadU32L(
        this ReadOnlySpan<byte> span,
        out uint val)
    {
        val = ReadUInt32LittleEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadS32L(
        this ReadOnlySpan<byte> span,
        out int val)
    {
        val = ReadInt32LittleEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadU32B(
        this ReadOnlySpan<byte> span,
        out uint val)
    {
        val = ReadUInt32BigEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadS32B(
        this ReadOnlySpan<byte> span,
        out int val)
    {
        val = ReadInt32BigEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadF32L(
        this ReadOnlySpan<byte> span,
        out float val)
    {
        val = ReadSingleLittleEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadF32B(
        this ReadOnlySpan<byte> span,
        out float val)
    {
        val = ReadSingleBigEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadU64L(
        this ReadOnlySpan<byte> span,
        out ulong val)
    {
        val = ReadUInt64LittleEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadS64L(
        this ReadOnlySpan<byte> span,
        out long val)
    {
        val = ReadInt64LittleEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadU64B(
        this ReadOnlySpan<byte> span,
        out ulong val)
    {
        val = ReadUInt64BigEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadS64B(
        this ReadOnlySpan<byte> span,
        out long val)
    {
        val = ReadInt64BigEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadF64L(
        this ReadOnlySpan<byte> span,
        out double val)
    {
        val = ReadDoubleLittleEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> ReadF64B(
        this ReadOnlySpan<byte> span,
        out double val)
    {
        val = ReadDoubleBigEndian(span);
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
        val = ReadUInt16LittleEndian(span);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteU16L(
        this Span<byte> span,
        ushort val)
    {
        WriteUInt16LittleEndian(span, val);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadS16L(
        this Span<byte> span,
        out short val)
    {
        val = ReadInt16LittleEndian(span);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteS16L(
        this Span<byte> span,
        short val)
    {
        WriteInt16LittleEndian(span, val);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadU16B(
        this Span<byte> span,
        out ushort val)
    {
        val = ReadUInt16BigEndian(span);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteU16B(
        this Span<byte> span,
        ushort val)
    {
        WriteUInt16BigEndian(span, val);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadS16B(
        this Span<byte> span,
        out short val)
    {
        val = ReadInt16BigEndian(span);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteS16B(
        this Span<byte> span,
        short val)
    {
        WriteInt16BigEndian(span, val);
        return span[2..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadU32L(
        this Span<byte> span,
        out uint val)
    {
        val = ReadUInt32LittleEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteU32L(
        this Span<byte> span,
        uint val)
    {
        WriteUInt32LittleEndian(span, val);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadS32L(
        this Span<byte> span,
        out int val)
    {
        val = ReadInt32LittleEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteS32L(
        this Span<byte> span,
        int val)
    {
        WriteInt32LittleEndian(span, val);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadU32B(
        this Span<byte> span,
        out uint val)
    {
        val = ReadUInt32BigEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteU32B(
        this Span<byte> span,
        uint val)
    {
        WriteUInt32BigEndian(span, val);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadS32B(
        this Span<byte> span,
        out int val)
    {
        val = ReadInt32BigEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteS32B(
        this Span<byte> span,
        int val)
    {
        WriteInt32BigEndian(span, val);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadF32L(
        this Span<byte> span,
        out float val)
    {
        val = ReadSingleLittleEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteF32L(
        this Span<byte> span,
        float val)
    {
        WriteSingleLittleEndian(span, val);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadF32B(
        this Span<byte> span,
        out float val)
    {
        val = ReadSingleBigEndian(span);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteF32B(
        this Span<byte> span,
        float val)
    {
        WriteSingleBigEndian(span, val);
        return span[4..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadU64L(
        this Span<byte> span,
        out ulong val)
    {
        val = ReadUInt64LittleEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteU64L(
        this Span<byte> span,
        ulong val)
    {
        WriteUInt64LittleEndian(span, val);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadS64L(
        this Span<byte> span,
        out long val)
    {
        val = ReadInt64LittleEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteS64L(
        this Span<byte> span,
        long val)
    {
        WriteInt64LittleEndian(span, val);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadU64B(
        this Span<byte> span,
        out ulong val)
    {
        val = ReadUInt64BigEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteU64B(
        this Span<byte> span,
        ulong val)
    {
        WriteUInt64BigEndian(span, val);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadS64B(
        this Span<byte> span,
        out long val)
    {
        val = ReadInt64BigEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteS64B(
        this Span<byte> span,
        long val)
    {
        WriteInt64BigEndian(span, val);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadF64L(
        this Span<byte> span,
        out double val)
    {
        val = ReadDoubleLittleEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteF64L(
        this Span<byte> span,
        double val)
    {
        WriteDoubleLittleEndian(span, val);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> ReadF64B(
        this Span<byte> span,
        out double val)
    {
        val = ReadDoubleBigEndian(span);
        return span[8..];
    }

    [DebuggerStepThrough]
    public static Span<byte> WriteF64B(
        this Span<byte> span,
        double val)
    {
        WriteDoubleBigEndian(span, val);
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
        ToU16L((ReadOnlySpan<byte>)span);

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
        ToU16B((ReadOnlySpan<byte>)span);

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
        ToS16L((ReadOnlySpan<byte>)span);

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
        ToS16B((ReadOnlySpan<byte>)span);

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
        ToU32L((ReadOnlySpan<byte>)span);

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
        ToU32B((ReadOnlySpan<byte>)span);

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
        ToS32L((ReadOnlySpan<byte>)span);

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
        ToS32B((ReadOnlySpan<byte>)span);

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
        ToF32L((ReadOnlySpan<byte>)span);

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
        ToF32B((ReadOnlySpan<byte>)span);

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
        ToU64L((ReadOnlySpan<byte>)span);

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
        ToU64B((ReadOnlySpan<byte>)span);

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
        ToS64L((ReadOnlySpan<byte>)span);

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
        ToS64B((ReadOnlySpan<byte>)span);

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
        ToF64L((ReadOnlySpan<byte>)span);

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
        ToF64B((ReadOnlySpan<byte>)span);

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
        ToU8L((ReadOnlySpan<ushort>)span);

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
        ToU8L((ReadOnlySpan<short>)span);

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
        ToU8L((ReadOnlySpan<uint>)span);

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
        ToU8L((ReadOnlySpan<int>)span);

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
        ToU8L((ReadOnlySpan<float>)span);

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
        ToU8L((ReadOnlySpan<ulong>)span);

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
        ToU8L((ReadOnlySpan<long>)span);

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
        ToU8L((ReadOnlySpan<double>)span);

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
        ToU8B((ReadOnlySpan<ushort>)span);

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
        ToU8B((ReadOnlySpan<short>)span);

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
        ToU8B((ReadOnlySpan<uint>)span);

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
        ToU8B((ReadOnlySpan<int>)span);

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
        ToU8B((ReadOnlySpan<float>)span);

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
        ToU8B((ReadOnlySpan<ulong>)span);

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
        ToU8B((ReadOnlySpan<long>)span);

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
        ToU8B((ReadOnlySpan<double>)span);

    #endregion

    // ***************************************************

    #region Interpretations

    [DebuggerStepThrough]
    public static ReadOnlySpan<ushort> CastU16L(
        this ReadOnlySpan<byte> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, ushort>(span)
            : ToU16L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<ushort> CastU16L(
        this Span<byte> span) =>
        CastU16L((ReadOnlySpan<byte>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<ushort> CastU16B(
        this ReadOnlySpan<byte> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, ushort>(span)
            : ToU16B(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<ushort> CastU16B(
        this Span<byte> span) =>
        CastU16B((ReadOnlySpan<byte>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<short> CastS16L(
        this ReadOnlySpan<byte> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, short>(span)
            : ToS16L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<short> CastS16L(
        this Span<byte> span) =>
        CastS16L((ReadOnlySpan<byte>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<short> CastS16B(
        this ReadOnlySpan<byte> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, short>(span)
            : ToS16B(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<short> CastS16B(
        this Span<byte> span) =>
        CastS16B((ReadOnlySpan<byte>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<uint> CastU32L(
        this ReadOnlySpan<byte> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, uint>(span)
            : ToU32L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<uint> CastU32L(
        this Span<byte> span) =>
        CastU32L((ReadOnlySpan<byte>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<uint> CastU32B(
        this ReadOnlySpan<byte> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, uint>(span)
            : ToU32B(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<uint> CastU32B(
        this Span<byte> span) =>
        CastU32B((ReadOnlySpan<byte>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<int> CastS32L(
        this ReadOnlySpan<byte> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, int>(span)
            : ToS32L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<int> CastS32L(
        this Span<byte> span) =>
        CastS32L((ReadOnlySpan<byte>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<int> CastS32B(
        this ReadOnlySpan<byte> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, int>(span)
            : ToS32B(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<int> CastS32B(
        this Span<byte> span) =>
        CastS32B((ReadOnlySpan<byte>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<float> CastF32L(
        this ReadOnlySpan<byte> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, float>(span)
            : ToF32L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<float> CastF32L(
        this Span<byte> span) =>
        CastF32L((ReadOnlySpan<byte>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<float> CastF32B(
        this ReadOnlySpan<byte> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, float>(span)
            : ToF32L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<float> CastF32B(
        this Span<byte> span) =>
        CastF32B((ReadOnlySpan<byte>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<ulong> CastU64L(
        this ReadOnlySpan<byte> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, ulong>(span)
            : ToU64L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<ulong> CastU64L(
        this Span<byte> span) =>
        CastU64L((ReadOnlySpan<byte>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<ulong> CastU64B(
        this ReadOnlySpan<byte> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, ulong>(span)
            : ToU64B(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<ulong> CastU64B(
        this Span<byte> span) =>
        CastU64B((ReadOnlySpan<byte>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<long> CastS64L(
        this ReadOnlySpan<byte> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, long>(span)
            : ToS64L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<long> CastS64L(
        this Span<byte> span) =>
        CastS64L((ReadOnlySpan<byte>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<long> CastS64B(
        this ReadOnlySpan<byte> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, long>(span)
            : ToS64B(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<long> CastS64B(
        this Span<byte> span) =>
        CastS64B((ReadOnlySpan<byte>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<double> CastF64L(
        this ReadOnlySpan<byte> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, double>(span)
            : ToF64L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<double> CastF64L(
        this Span<byte> span) =>
        CastF64L((ReadOnlySpan<byte>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<double> CastF64B(
        this ReadOnlySpan<byte> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<byte, double>(span)
            : ToF64B(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<double> CastF64B(
        this Span<byte> span) =>
        CastF64B((ReadOnlySpan<byte>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this ReadOnlySpan<ushort> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<ushort, byte>(span)
            : ToU8L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this Span<ushort> span) =>
        CastU8L((ReadOnlySpan<ushort>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this ReadOnlySpan<short> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<short, byte>(span)
            : ToU8L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this Span<short> span) =>
        CastU8L((ReadOnlySpan<short>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this ReadOnlySpan<uint> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<uint, byte>(span)
            : ToU8L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this Span<uint> span) =>
        CastU8L((ReadOnlySpan<uint>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this ReadOnlySpan<int> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<int, byte>(span)
            : ToU8L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this Span<int> span) =>
        CastU8L((ReadOnlySpan<int>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this ReadOnlySpan<float> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<float, byte>(span)
            : ToU8L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this Span<float> span) =>
        CastU8L((ReadOnlySpan<float>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this ReadOnlySpan<ulong> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<ulong, byte>(span)
            : ToU8L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this Span<ulong> span) =>
        CastU8L((ReadOnlySpan<ulong>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this ReadOnlySpan<long> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<long, byte>(span)
            : ToU8L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this Span<long> span) =>
        CastU8L((ReadOnlySpan<long>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this ReadOnlySpan<double> span) =>
        BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<double, byte>(span)
            : ToU8L(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(
        this Span<double> span) =>
        CastU8L((ReadOnlySpan<double>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this ReadOnlySpan<ushort> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<ushort, byte>(span)
            : ToU8B(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this Span<ushort> span) =>
        CastU8B((ReadOnlySpan<ushort>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this ReadOnlySpan<short> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<short, byte>(span)
            : ToU8B(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this Span<short> span) =>
        CastU8B((ReadOnlySpan<short>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this ReadOnlySpan<uint> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<uint, byte>(span)
            : ToU8B(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this Span<uint> span) =>
        CastU8B((ReadOnlySpan<uint>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this ReadOnlySpan<int> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<int, byte>(span)
            : ToU8B(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this Span<int> span) =>
        CastU8B((ReadOnlySpan<int>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this ReadOnlySpan<float> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<float, byte>(span)
            : ToU8B(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this Span<float> span) =>
        CastU8B((ReadOnlySpan<float>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this ReadOnlySpan<ulong> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<ulong, byte>(span)
            : ToU8B(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this Span<ulong> span) =>
        CastU8B((ReadOnlySpan<ulong>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this ReadOnlySpan<long> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<long, byte>(span)
            : ToU8B(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this Span<long> span) =>
        CastU8B((ReadOnlySpan<long>)span);

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this ReadOnlySpan<double> span) =>
        !BitConverter.IsLittleEndian
            ? MemoryMarshal.Cast<double, byte>(span)
            : ToU8B(span).Span;

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(
        this Span<double> span) =>
        CastU8B((ReadOnlySpan<double>)span);

    #endregion
    
    // ***************************************************
}