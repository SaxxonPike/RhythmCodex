using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace RhythmCodex.Utils.Cursors;

[PublicAPI]
public static class ArrayExtensions
{
    // ***************************************************

    #region Array

    extension(byte[] arr)
    {
        [DebuggerStepThrough]
        public Memory<byte> ReadU8(out byte val) =>
            arr.AsMemory().ReadU8(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteU8(byte val) =>
            arr.AsMemory().WriteU8(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadS8(out sbyte val) =>
            arr.AsMemory().ReadS8(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteS8(sbyte val) =>
            arr.AsMemory().WriteS8(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadU16L(out ushort val) =>
            arr.AsMemory().ReadU16L(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteU16L(ushort val) =>
            arr.AsMemory().WriteU16L(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadS16L(out short val) =>
            arr.AsMemory().ReadS16L(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteS16L(short val) =>
            arr.AsMemory().WriteS16L(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadU16B(out ushort val) =>
            arr.AsMemory().ReadU16B(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteU16B(ushort val) =>
            arr.AsMemory().WriteU16B(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadS16B(out short val) =>
            arr.AsMemory().ReadS16B(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteS16B(short val) =>
            arr.AsMemory().WriteS16B(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadU32L(out uint val) =>
            arr.AsMemory().ReadU32L(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteU32L(uint val) =>
            arr.AsMemory().WriteU32L(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadS32L(out int val) =>
            arr.AsMemory().ReadS32L(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteS32L(int val) =>
            arr.AsMemory().WriteS32L(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadU32B(out uint val) =>
            arr.AsMemory().ReadU32B(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteU32B(uint val) =>
            arr.AsMemory().WriteU32B(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadS32B(out int val) =>
            arr.AsMemory().ReadS32B(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteS32B(int val) =>
            arr.AsMemory().WriteS32B(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadF32L(out float val) =>
            arr.AsMemory().ReadF32L(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteF32L(float val) =>
            arr.AsMemory().WriteF32L(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadF32B(out float val) =>
            arr.AsMemory().ReadF32B(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteF32B(float val) =>
            arr.AsMemory().WriteF32B(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadU64L(out ulong val) =>
            arr.AsMemory().ReadU64L(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteU64L(ulong val) =>
            arr.AsMemory().WriteU64L(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadS64L(out long val) =>
            arr.AsMemory().ReadS64L(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteS64L(long val) =>
            arr.AsMemory().WriteS64L(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadU64B(out ulong val) =>
            arr.AsMemory().ReadU64B(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteU64B(ulong val) =>
            arr.AsMemory().WriteU64B(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadS64B(out long val) =>
            arr.AsMemory().ReadS64B(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteS64B(long val) =>
            arr.AsMemory().WriteS64B(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadF64L(out double val) =>
            arr.AsMemory().ReadF64L(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteF64L(double val) =>
            arr.AsMemory().WriteF64L(val);

        [DebuggerStepThrough]
        public Memory<byte> ReadF64B(out double val) =>
            arr.AsMemory().ReadF64B(out val);

        [DebuggerStepThrough]
        public Memory<byte> WriteF64B(double val) =>
            arr.AsMemory().WriteF64B(val);

        [DebuggerStepThrough]
        public Memory<byte> Skip([NonNegativeValue] int count) =>
            arr.AsMemory().Skip(count);

        [DebuggerStepThrough]
        public Memory<byte> Extract([NonNegativeValue] int count,
            out Memory<byte> val) =>
            arr.AsMemory().Extract(count, out val);

        [DebuggerStepThrough]
        public Memory<byte> TryExtract([NonNegativeValue] int count,
            out Memory<byte> val) =>
            arr.AsMemory().TryExtract(count, out val);

        [DebuggerStepThrough]
        public Memory<byte> Read([NonNegativeValue] int count,
            out byte[] val) =>
            arr.AsMemory().Read(count, out val);

        [DebuggerStepThrough]
        public Memory<byte> Read(Memory<byte> target,
            [NonNegativeValue] out int count) =>
            arr.AsMemory().Read(target, out count);

        [DebuggerStepThrough]
        public Memory<byte> Read(Span<byte> target,
            [NonNegativeValue] out int count) =>
            arr.AsMemory().Read(target, out count);

        [DebuggerStepThrough]
        public Memory<byte> TryRead([NonNegativeValue] int count,
            out byte[] val) =>
            arr.AsMemory().TryRead(count, out val);

        [DebuggerStepThrough]
        public Memory<byte> TryRead(Memory<byte> target,
            [NonNegativeValue] out int count) =>
            arr.AsMemory().TryRead(target, out count);

        [DebuggerStepThrough]
        public Memory<byte> TryRead(Span<byte> target,
            [NonNegativeValue] out int count) =>
            arr.AsMemory().TryRead(target, out count);

        [DebuggerStepThrough]
        public Memory<byte> Write(ReadOnlySpan<byte> val,
            out int count) =>
            arr.AsMemory().Write(val, out count);

        [DebuggerStepThrough]
        public Memory<byte> Write(ReadOnlyMemory<byte> val,
            out int count) =>
            arr.AsMemory().Write(val, out count);

        [DebuggerStepThrough]
        public Memory<byte> TryWrite(ReadOnlySpan<byte> val,
            out int count) =>
            arr.AsMemory().Write(val, out count);

        [DebuggerStepThrough]
        public Memory<byte> TryWrite(ReadOnlyMemory<byte> val,
            out int count) =>
            arr.AsMemory().Write(val, out count);
    }

    #endregion

    // ***************************************************

    #region Conversions

    extension(byte[] arr)
    {
        [DebuggerStepThrough]
        public Memory<ushort> ToU16L() =>
            arr.AsSpan().ToU16L();

        [DebuggerStepThrough]
        public Memory<ushort> ToU16B() =>
            arr.AsSpan().ToU16B();

        [DebuggerStepThrough]
        public Memory<short> ToS16L() =>
            arr.AsSpan().ToS16L();

        [DebuggerStepThrough]
        public Memory<short> ToS16B() =>
            arr.AsSpan().ToS16B();

        [DebuggerStepThrough]
        public Memory<uint> ToU32L() =>
            arr.AsSpan().ToU32L();

        [DebuggerStepThrough]
        public Memory<uint> ToU32B() =>
            arr.AsSpan().ToU32B();

        [DebuggerStepThrough]
        public Memory<int> ToS32L() =>
            arr.AsSpan().ToS32L();

        [DebuggerStepThrough]
        public Memory<int> ToS32B() =>
            arr.AsSpan().ToS32B();

        [DebuggerStepThrough]
        public Memory<float> ToF32L() =>
            arr.AsSpan().ToF32L();

        [DebuggerStepThrough]
        public Memory<float> ToF32B() =>
            arr.AsSpan().ToF32B();

        [DebuggerStepThrough]
        public Memory<ulong> ToU64L() =>
            arr.AsSpan().ToU64L();

        [DebuggerStepThrough]
        public Memory<ulong> ToU64B() =>
            arr.AsSpan().ToU64B();

        [DebuggerStepThrough]
        public Memory<long> ToS64L() =>
            arr.AsSpan().ToS64L();

        [DebuggerStepThrough]
        public Memory<long> ToS64B() =>
            arr.AsSpan().ToS64B();

        [DebuggerStepThrough]
        public Memory<double> ToF64L() =>
            arr.AsSpan().ToF64L();

        [DebuggerStepThrough]
        public Memory<double> ToF64B() =>
            arr.AsSpan().ToF64B();
    }

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

    extension(byte[] arr)
    {
        [DebuggerStepThrough]
        public ReadOnlySpan<ushort> CastU16L() =>
            arr.AsSpan().CastU16L();

        [DebuggerStepThrough]
        public ReadOnlySpan<ushort> CastU16B() =>
            arr.AsSpan().CastU16B();

        [DebuggerStepThrough]
        public ReadOnlySpan<short> CastS16L() =>
            arr.AsSpan().CastS16L();

        [DebuggerStepThrough]
        public ReadOnlySpan<short> CastS16B() =>
            arr.AsSpan().CastS16B();

        [DebuggerStepThrough]
        public ReadOnlySpan<uint> CastU32L() =>
            arr.AsSpan().CastU32L();

        [DebuggerStepThrough]
        public ReadOnlySpan<uint> CastU32B() =>
            arr.AsSpan().CastU32B();

        [DebuggerStepThrough]
        public ReadOnlySpan<int> CastS32L() =>
            arr.AsSpan().CastS32L();

        [DebuggerStepThrough]
        public ReadOnlySpan<int> CastS32B() =>
            arr.AsSpan().CastS32B();

        [DebuggerStepThrough]
        public ReadOnlySpan<float> CastF32L() =>
            arr.AsSpan().CastF32L();

        [DebuggerStepThrough]
        public ReadOnlySpan<float> CastF32B() =>
            arr.AsSpan().CastF32B();

        [DebuggerStepThrough]
        public ReadOnlySpan<ulong> CastU64L() =>
            arr.AsSpan().CastU64L();

        [DebuggerStepThrough]
        public ReadOnlySpan<ulong> CastU64B() =>
            arr.AsSpan().CastU64B();

        [DebuggerStepThrough]
        public ReadOnlySpan<long> CastS64L() =>
            arr.AsSpan().CastS64L();

        [DebuggerStepThrough]
        public ReadOnlySpan<long> CastS64B() =>
            arr.AsSpan().CastS64B();

        [DebuggerStepThrough]
        public ReadOnlySpan<double> CastF64L() =>
            arr.AsSpan().CastF64L();

        [DebuggerStepThrough]
        public ReadOnlySpan<double> CastF64B() =>
            arr.AsSpan().CastF64B();
    }

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