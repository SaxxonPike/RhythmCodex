using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace RhythmCodex.Utils.Cursors;

[PublicAPI]
public static class ArrayExtensions
{
    // ***************************************************

    #region Array

    [DebuggerStepThrough]
    public static Memory<byte> ReadU8(
        this byte[] arr,
        out byte val) =>
        arr.AsMemory().ReadU8(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteU8(
        this byte[] arr,
        byte val) =>
        arr.AsMemory().WriteU8(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadS8(
        this byte[] arr,
        out sbyte val) =>
        arr.AsMemory().ReadS8(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteS8(
        this byte[] arr,
        sbyte val) =>
        arr.AsMemory().WriteS8(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadU16L(
        this byte[] arr,
        out ushort val) =>
        arr.AsMemory().ReadU16L(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteU16L(
        this byte[] arr,
        ushort val) =>
        arr.AsMemory().WriteU16L(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadS16L(
        this byte[] arr,
        out short val) =>
        arr.AsMemory().ReadS16L(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteS16L(
        this byte[] arr,
        short val) =>
        arr.AsMemory().WriteS16L(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadU16B(
        this byte[] arr,
        out ushort val) =>
        arr.AsMemory().ReadU16B(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteU16B(
        this byte[] arr,
        ushort val) =>
        arr.AsMemory().WriteU16B(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadS16B(
        this byte[] arr,
        out short val) =>
        arr.AsMemory().ReadS16B(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteS16B(
        this byte[] arr,
        short val) =>
        arr.AsMemory().WriteS16B(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadU32L(
        this byte[] arr,
        out uint val) =>
        arr.AsMemory().ReadU32L(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteU32L(
        this byte[] arr,
        uint val) =>
        arr.AsMemory().WriteU32L(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadS32L(
        this byte[] arr,
        out int val) =>
        arr.AsMemory().ReadS32L(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteS32L(
        this byte[] arr,
        int val) =>
        arr.AsMemory().WriteS32L(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadU32B(
        this byte[] arr,
        out uint val) =>
        arr.AsMemory().ReadU32B(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteU32B(
        this byte[] arr,
        uint val) =>
        arr.AsMemory().WriteU32B(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadS32B(
        this byte[] arr,
        out int val) =>
        arr.AsMemory().ReadS32B(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteS32B(
        this byte[] arr,
        int val) =>
        arr.AsMemory().WriteS32B(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadF32L(
        this byte[] arr,
        out float val) =>
        arr.AsMemory().ReadF32L(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteF32L(
        this byte[] arr,
        float val) =>
        arr.AsMemory().WriteF32L(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadF32B(
        this byte[] arr,
        out float val) =>
        arr.AsMemory().ReadF32B(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteF32B(
        this byte[] arr,
        float val) =>
        arr.AsMemory().WriteF32B(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadU64L(
        this byte[] arr,
        out ulong val) =>
        arr.AsMemory().ReadU64L(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteU64L(
        this byte[] arr,
        ulong val) =>
        arr.AsMemory().WriteU64L(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadS64L(
        this byte[] arr,
        out long val) =>
        arr.AsMemory().ReadS64L(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteS64L(
        this byte[] arr,
        long val) =>
        arr.AsMemory().WriteS64L(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadU64B(
        this byte[] arr,
        out ulong val) =>
        arr.AsMemory().ReadU64B(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteU64B(
        this byte[] arr,
        ulong val) =>
        arr.AsMemory().WriteU64B(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadS64B(
        this byte[] arr,
        out long val) =>
        arr.AsMemory().ReadS64B(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteS64B(
        this byte[] arr,
        long val) =>
        arr.AsMemory().WriteS64B(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadF64L(
        this byte[] arr,
        out double val) =>
        arr.AsMemory().ReadF64L(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteF64L(
        this byte[] arr,
        double val) =>
        arr.AsMemory().WriteF64L(val);

    [DebuggerStepThrough]
    public static Memory<byte> ReadF64B(
        this byte[] arr,
        out double val) =>
        arr.AsMemory().ReadF64B(out val);

    [DebuggerStepThrough]
    public static Memory<byte> WriteF64B(
        this byte[] arr,
        double val) =>
        arr.AsMemory().WriteF64B(val);

    [DebuggerStepThrough]
    public static Memory<byte> Skip(
        this byte[] arr,
        [NonNegativeValue] int count) =>
        arr.AsMemory().Skip(count);

    [DebuggerStepThrough]
    public static Memory<byte> Extract(
        this byte[] arr,
        [NonNegativeValue] int count,
        out Memory<byte> val) =>
        arr.AsMemory().Extract(count, out val);

    [DebuggerStepThrough]
    public static Memory<byte> TryExtract(
        this byte[] arr,
        [NonNegativeValue] int count,
        out Memory<byte> val) =>
        arr.AsMemory().TryExtract(count, out val);

    [DebuggerStepThrough]
    public static Memory<byte> Read(
        this byte[] arr,
        [NonNegativeValue] int count,
        out byte[] val) =>
        arr.AsMemory().Read(count, out val);

    [DebuggerStepThrough]
    public static Memory<byte> Read(
        this byte[] arr,
        Memory<byte> target,
        [NonNegativeValue] out int count) =>
        arr.AsMemory().Read(target, out count);

    [DebuggerStepThrough]
    public static Memory<byte> Read(
        this byte[] arr,
        Span<byte> target,
        [NonNegativeValue] out int count) =>
        arr.AsMemory().Read(target, out count);

    [DebuggerStepThrough]
    public static Memory<byte> TryRead(
        this byte[] arr,
        [NonNegativeValue] int count,
        out byte[] val) =>
        arr.AsMemory().TryRead(count, out val);

    [DebuggerStepThrough]
    public static Memory<byte> TryRead(
        this byte[] arr,
        Memory<byte> target,
        [NonNegativeValue] out int count) =>
        arr.AsMemory().TryRead(target, out count);

    [DebuggerStepThrough]
    public static Memory<byte> TryRead(
        this byte[] arr,
        Span<byte> target,
        [NonNegativeValue] out int count) =>
        arr.AsMemory().TryRead(target, out count);

    [DebuggerStepThrough]
    public static Memory<byte> Write(
        this byte[] arr,
        ReadOnlySpan<byte> val,
        out int count) =>
        arr.AsMemory().Write(val, out count);

    [DebuggerStepThrough]
    public static Memory<byte> Write(
        this byte[] arr,
        ReadOnlyMemory<byte> val,
        out int count) =>
        arr.AsMemory().Write(val, out count);

    [DebuggerStepThrough]
    public static Memory<byte> TryWrite(
        this byte[] arr,
        ReadOnlySpan<byte> val,
        out int count) =>
        arr.AsMemory().Write(val, out count);

    [DebuggerStepThrough]
    public static Memory<byte> TryWrite(
        this byte[] arr,
        ReadOnlyMemory<byte> val,
        out int count) =>
        arr.AsMemory().Write(val, out count);


    #endregion

    // ***************************************************

    #region Conversions

    [DebuggerStepThrough]
    public static Memory<ushort> ToU16L(this byte[] arr) =>
        arr.AsSpan().ToU16L();

    [DebuggerStepThrough]
    public static Memory<ushort> ToU16B(this byte[] arr) =>
        arr.AsSpan().ToU16B();

    [DebuggerStepThrough]
    public static Memory<short> ToS16L(this byte[] arr) =>
        arr.AsSpan().ToS16L();

    [DebuggerStepThrough]
    public static Memory<short> ToS16B(this byte[] arr) =>
        arr.AsSpan().ToS16B();

    [DebuggerStepThrough]
    public static Memory<uint> ToU32L(this byte[] arr) =>
        arr.AsSpan().ToU32L();

    [DebuggerStepThrough]
    public static Memory<uint> ToU32B(this byte[] arr) =>
        arr.AsSpan().ToU32B();

    [DebuggerStepThrough]
    public static Memory<int> ToS32L(this byte[] arr) =>
        arr.AsSpan().ToS32L();

    [DebuggerStepThrough]
    public static Memory<int> ToS32B(this byte[] arr) =>
        arr.AsSpan().ToS32B();

    [DebuggerStepThrough]
    public static Memory<float> ToF32L(this byte[] arr) =>
        arr.AsSpan().ToF32L();

    [DebuggerStepThrough]
    public static Memory<float> ToF32B(this byte[] arr) =>
        arr.AsSpan().ToF32B();

    [DebuggerStepThrough]
    public static Memory<ulong> ToU64L(this byte[] arr) =>
        arr.AsSpan().ToU64L();

    [DebuggerStepThrough]
    public static Memory<ulong> ToU64B(this byte[] arr) =>
        arr.AsSpan().ToU64B();

    [DebuggerStepThrough]
    public static Memory<long> ToS64L(this byte[] arr) =>
        arr.AsSpan().ToS64L();

    [DebuggerStepThrough]
    public static Memory<long> ToS64B(this byte[] arr) =>
        arr.AsSpan().ToS64B();

    [DebuggerStepThrough]
    public static Memory<double> ToF64L(this byte[] arr) =>
        arr.AsSpan().ToF64L();

    [DebuggerStepThrough]
    public static Memory<double> ToF64B(this byte[] arr) =>
        arr.AsSpan().ToF64B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this ushort[] arr) =>
        arr.AsSpan().ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this short[] arr) =>
        arr.AsSpan().ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this uint[] arr) =>
        arr.AsSpan().ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this int[] arr) =>
        arr.AsSpan().ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this float[] arr) =>
        arr.AsSpan().ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this ulong[] arr) =>
        arr.AsSpan().ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this long[] arr) =>
        arr.AsSpan().ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8L(this double[] arr) =>
        arr.AsSpan().ToU8L();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this ushort[] arr) =>
        arr.AsSpan().ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this short[] arr) =>
        arr.AsSpan().ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this uint[] arr) =>
        arr.AsSpan().ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this int[] arr) =>
        arr.AsSpan().ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this float[] arr) =>
        arr.AsSpan().ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this ulong[] arr) =>
        arr.AsSpan().ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this long[] arr) =>
        arr.AsSpan().ToU8B();

    [DebuggerStepThrough]
    public static Memory<byte> ToU8B(this double[] arr) =>
        arr.AsSpan().ToU8B();

    #endregion

    // ***************************************************

    #region Interpretations

    [DebuggerStepThrough]
    public static ReadOnlySpan<ushort> CastU16L(this byte[] arr) =>
        arr.AsSpan().CastU16L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<ushort> CastU16B(this byte[] arr) =>
        arr.AsSpan().CastU16B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<short> CastS16L(this byte[] arr) =>
        arr.AsSpan().CastS16L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<short> CastS16B(this byte[] arr) =>
        arr.AsSpan().CastS16B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<uint> CastU32L(this byte[] arr) =>
        arr.AsSpan().CastU32L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<uint> CastU32B(this byte[] arr) =>
        arr.AsSpan().CastU32B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<int> CastS32L(this byte[] arr) =>
        arr.AsSpan().CastS32L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<int> CastS32B(this byte[] arr) =>
        arr.AsSpan().CastS32B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<float> CastF32L(this byte[] arr) =>
        arr.AsSpan().CastF32L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<float> CastF32B(this byte[] arr) =>
        arr.AsSpan().CastF32B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<ulong> CastU64L(this byte[] arr) =>
        arr.AsSpan().CastU64L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<ulong> CastU64B(this byte[] arr) =>
        arr.AsSpan().CastU64B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<long> CastS64L(this byte[] arr) =>
        arr.AsSpan().CastS64L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<long> CastS64B(this byte[] arr) =>
        arr.AsSpan().CastS64B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<double> CastF64L(this byte[] arr) =>
        arr.AsSpan().CastF64L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<double> CastF64B(this byte[] arr) =>
        arr.AsSpan().CastF64B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this ushort[] arr) =>
        arr.AsSpan().CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this short[] arr) =>
        arr.AsSpan().CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this uint[] arr) =>
        arr.AsSpan().CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this int[] arr) =>
        arr.AsSpan().CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this float[] arr) =>
        arr.AsSpan().CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this ulong[] arr) =>
        arr.AsSpan().CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this long[] arr) =>
        arr.AsSpan().CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8L(this double[] arr) =>
        arr.AsSpan().CastU8L();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this ushort[] arr) =>
        arr.AsSpan().CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this short[] arr) =>
        arr.AsSpan().CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this uint[] arr) =>
        arr.AsSpan().CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this int[] arr) =>
        arr.AsSpan().CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this float[] arr) =>
        arr.AsSpan().CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this ulong[] arr) =>
        arr.AsSpan().CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this long[] arr) =>
        arr.AsSpan().CastU8B();

    [DebuggerStepThrough]
    public static ReadOnlySpan<byte> CastU8B(this double[] arr) =>
        arr.AsSpan().CastU8B();

    #endregion

    // ***************************************************
}