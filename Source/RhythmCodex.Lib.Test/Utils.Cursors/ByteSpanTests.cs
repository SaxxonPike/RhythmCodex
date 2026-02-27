using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace RhythmCodex.Utils.Cursors;

[TestFixture]
[Parallelizable(ParallelScope.None)]
public class ByteSpanTests : BaseTestFixture
{
    private byte[] _bytes;

    private static IEnumerable<object> WithinBoundsCounts() =>
    [
        new object[] { 0 },
        new object[] { 1 },
        new object[] { 8 }
    ];

    private static IEnumerable<object> OutOfBoundsCounts() =>
    [
        new object[] { 0 },
        new object[] { 8 },
        new object[] { 256 }
    ];

    [SetUp]
    public void SetUp()
    {
        var bytes = new byte[16];
        Random.NextBytes(bytes);
        _bytes = bytes;
    }

    private void AssertBytes(byte[] expected)
    {
        _bytes.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadU8()
    {
        var expected = _bytes[0];
        var span = _bytes.AsSpan();

        Assert.That(span.ReadU8(out var observed).SequenceEqual(span[1..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadU8_ReadOnly()
    {
        var expected = _bytes[0];
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadU8(out var observed).SequenceEqual(span[1..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU8()
    {
        var val = Random.NextByte();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        expected[0] = val;

        Assert.That(span.WriteU8(val).SequenceEqual(span[1..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadS8()
    {
        var expected = unchecked((sbyte)_bytes[0]);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadS8(out var observed).SequenceEqual(span[1..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadS8_ReadOnly()
    {
        var expected = unchecked((sbyte)_bytes[0]);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadS8(out var observed).SequenceEqual(span[1..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS8()
    {
        var val = Random.NextSByte();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        expected[0] = unchecked((byte)val);

        Assert.That(span.WriteS8(val).SequenceEqual(span[1..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadU16L()
    {
        var expected = ReadUInt16LittleEndian(_bytes);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadU16L(out var observed).SequenceEqual(span[2..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadU16L_ReadOnly()
    {
        var expected = ReadUInt16LittleEndian(_bytes);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadU16L(out var observed).SequenceEqual(span[2..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU16L()
    {
        var val = Random.NextUShort();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        WriteUInt16LittleEndian(expected, val);

        Assert.That(span.WriteU16L(val).SequenceEqual(span[2..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadU16B()
    {
        var expected = ReadUInt16BigEndian(_bytes);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadU16B(out var observed).SequenceEqual(span[2..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadU16B_ReadOnly()
    {
        var expected = ReadUInt16BigEndian(_bytes);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadU16B(out var observed).SequenceEqual(span[2..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU16B()
    {
        var val = Random.NextUShort();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        WriteUInt16BigEndian(expected, val);

        Assert.That(span.WriteU16B(val).SequenceEqual(span[2..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadS16L()
    {
        var expected = ReadInt16LittleEndian(_bytes);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadS16L(out var observed).SequenceEqual(span[2..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadS16L_ReadOnly()
    {
        var expected = ReadInt16LittleEndian(_bytes);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadS16L(out var observed).SequenceEqual(span[2..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS16L()
    {
        var val = Random.NextShort();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        WriteInt16LittleEndian(expected, val);

        Assert.That(span.WriteS16L(val).SequenceEqual(span[2..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadS16B()
    {
        var expected = ReadInt16BigEndian(_bytes);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadS16B(out var observed).SequenceEqual(span[2..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadS16B_ReadOnly()
    {
        var expected = ReadInt16BigEndian(_bytes);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadS16B(out var observed).SequenceEqual(span[2..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS16B()
    {
        var val = Random.NextShort();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        WriteInt16BigEndian(expected, val);

        Assert.That(span.WriteS16B(val).SequenceEqual(span[2..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadU32L()
    {
        var expected = ReadUInt32LittleEndian(_bytes);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadU32L(out var observed).SequenceEqual(span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadU32L_ReadOnly()
    {
        var expected = ReadUInt32LittleEndian(_bytes);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadU32L(out var observed).SequenceEqual(span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU32L()
    {
        var val = Random.NextUShort();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        WriteUInt32LittleEndian(expected, val);

        Assert.That(span.WriteU32L(val).SequenceEqual(span[4..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadU32B()
    {
        var expected = ReadUInt32BigEndian(_bytes);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadU32B(out var observed).SequenceEqual(span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadU32B_ReadOnly()
    {
        var expected = ReadUInt32BigEndian(_bytes);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadU32B(out var observed).SequenceEqual(span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU32B()
    {
        var val = Random.NextUShort();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        WriteUInt32BigEndian(expected, val);

        Assert.That(span.WriteU32B(val).SequenceEqual(span[4..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadS32L()
    {
        var expected = ReadInt32LittleEndian(_bytes);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadS32L(out var observed).SequenceEqual(span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadS32L_ReadOnly()
    {
        var expected = ReadInt32LittleEndian(_bytes);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadS32L(out var observed).SequenceEqual(span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS32L()
    {
        var val = Random.NextShort();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        WriteInt32LittleEndian(expected, val);

        Assert.That(span.WriteS32L(val).SequenceEqual(span[4..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadS32B()
    {
        var expected = ReadInt32BigEndian(_bytes);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadS32B(out var observed).SequenceEqual(span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadS32B_ReadOnly()
    {
        var expected = ReadInt32BigEndian(_bytes);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadS32B(out var observed).SequenceEqual(span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS32B()
    {
        var val = Random.NextShort();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        WriteInt32BigEndian(expected, val);

        Assert.That(span.WriteS32B(val).SequenceEqual(span[4..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadF32L()
    {
        var expected = ReadSingleLittleEndian(_bytes);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadF32L(out var observed).SequenceEqual(span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadF32L_ReadOnly()
    {
        var expected = ReadSingleLittleEndian(_bytes);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadF32L(out var observed).SequenceEqual(span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteF32L()
    {
        var val = Random.NextShort();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        WriteSingleLittleEndian(expected, val);

        Assert.That(span.WriteF32L(val).SequenceEqual(span[4..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadF32B()
    {
        var expected = ReadSingleBigEndian(_bytes);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadF32B(out var observed).SequenceEqual(span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadF32B_ReadOnly()
    {
        var expected = ReadSingleBigEndian(_bytes);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadF32B(out var observed).SequenceEqual(span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteF32B()
    {
        var val = Random.NextShort();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        WriteSingleBigEndian(expected, val);

        Assert.That(span.WriteF32B(val).SequenceEqual(span[4..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadU64L()
    {
        var expected = ReadUInt64LittleEndian(_bytes);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadU64L(out var observed).SequenceEqual(span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadU64L_ReadOnly()
    {
        var expected = ReadUInt64LittleEndian(_bytes);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadU64L(out var observed).SequenceEqual(span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU64L()
    {
        var val = Random.NextUShort();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        WriteUInt64LittleEndian(expected, val);

        Assert.That(span.WriteU64L(val).SequenceEqual(span[8..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadU64B()
    {
        var expected = ReadUInt64BigEndian(_bytes);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadU64B(out var observed).SequenceEqual(span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadU64B_ReadOnly()
    {
        var expected = ReadUInt64BigEndian(_bytes);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadU64B(out var observed).SequenceEqual(span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU64B()
    {
        var val = Random.NextUShort();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        WriteUInt64BigEndian(expected, val);

        Assert.That(span.WriteU64B(val).SequenceEqual(span[8..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadS64L()
    {
        var expected = ReadInt64LittleEndian(_bytes);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadS64L(out var observed).SequenceEqual(span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadS64L_ReadOnly()
    {
        var expected = ReadInt64LittleEndian(_bytes);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadS64L(out var observed).SequenceEqual(span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS64L()
    {
        var val = Random.NextShort();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        WriteInt64LittleEndian(expected, val);

        Assert.That(span.WriteS64L(val).SequenceEqual(span[8..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadS64B()
    {
        var expected = ReadInt64BigEndian(_bytes);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadS64B(out var observed).SequenceEqual(span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadS64B_ReadOnly()
    {
        var expected = ReadInt64BigEndian(_bytes);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadS64B(out var observed).SequenceEqual(span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS64B()
    {
        var val = Random.NextShort();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        WriteInt64BigEndian(expected, val);

        Assert.That(span.WriteS64B(val).SequenceEqual(span[8..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadF64L()
    {
        var expected = ReadDoubleLittleEndian(_bytes);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadF64L(out var observed).SequenceEqual(span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadF64L_ReadOnly()
    {
        var expected = ReadDoubleLittleEndian(_bytes);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadF64L(out var observed).SequenceEqual(span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteF64L()
    {
        var val = Random.NextShort();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        WriteDoubleLittleEndian(expected, val);

        Assert.That(span.WriteF64L(val).SequenceEqual(span[8..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadF64B()
    {
        var expected = ReadDoubleBigEndian(_bytes);
        var span = _bytes.AsSpan();

        Assert.That(span.ReadF64B(out var observed).SequenceEqual(span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadF64B_ReadOnly()
    {
        var expected = ReadDoubleBigEndian(_bytes);
        ReadOnlySpan<byte> span = _bytes.AsSpan();

        Assert.That(span.ReadF64B(out var observed).SequenceEqual(span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteF64B()
    {
        var val = Random.NextShort();
        var span = _bytes.AsSpan();
        var expected = _bytes.ToArray();
        WriteDoubleBigEndian(expected, val);

        Assert.That(span.WriteF64B(val).SequenceEqual(span[8..]));
        AssertBytes(expected);
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Skip(int count)
    {
        var span = _bytes.AsSpan();
        Assert.That(span.Skip(count).SequenceEqual(span[count..]));
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Skip_ReadOnly(int count)
    {
        ReadOnlySpan<byte> span = _bytes.AsSpan();
        Assert.That(span.Skip(count).SequenceEqual(span[count..]));
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Extract(int count)
    {
        Assert.Multiple(() =>
        {
            var span = _bytes.AsSpan();
            Assert.That(span.Extract(count, out var val).SequenceEqual(span[count..]));
            Assert.That(val.SequenceEqual(span[..count]));
        });
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Extract_ReadOnly(int count)
    {
        Assert.Multiple(() =>
        {
            ReadOnlySpan<byte> span = _bytes.AsSpan();
            Assert.That(span.Extract(count, out var val).SequenceEqual(span[count..]));
            Assert.That(val.SequenceEqual(span[..count]));
        });
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryExtract(int count)
    {
        Assert.Multiple(() =>
        {
            var span = _bytes.AsSpan();
            var size = Math.Min(count, span.Length);
            var remainder = span[size..];
            Assert.That(span.TryExtract(count, out var val).SequenceEqual(remainder));
            Assert.That(val.SequenceEqual(span[..size]));
        });
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryExtract_ReadOnly(int count)
    {
        Assert.Multiple(() =>
        {
            ReadOnlySpan<byte> span = _bytes.AsSpan();
            var size = Math.Min(count, span.Length);
            var remainder = span[Math.Min(count, span.Length)..];
            Assert.That(span.TryExtract(count, out var val).SequenceEqual(remainder));
            Assert.That(val.SequenceEqual(span[..size]));
        });
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Read_Array(int count)
    {
        Assert.Multiple(() =>
        {
            var span = _bytes.AsSpan();
            var remainder = span[Math.Min(count, span.Length)..];
            Assert.That(span.Read(count, out var val).SequenceEqual(remainder));
            Assert.That(val.AsSpan().SequenceEqual(span[..count]));
        });
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Read_Array_ReadOnly(int count)
    {
        Assert.Multiple(() =>
        {
            ReadOnlySpan<byte> span = _bytes.AsSpan();
            var remainder = span[Math.Min(count, span.Length)..];
            Assert.That(span.Read(count, out var val).SequenceEqual(remainder));
            Assert.That(val.AsSpan().SequenceEqual(span[..count]));
        });
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Read_IntoSpan(int count)
    {
        var span = _bytes.AsSpan();
        var target = new byte[count * 2];
        var expected = new byte[target.Length];
        _bytes.AsSpan(0, count).CopyTo(expected);

        Assert.That(span.Read(target.AsSpan(0, count), out var val).SequenceEqual(span[count..]));
        target.ShouldBe(expected);
        val.ShouldBe(count);
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Read_IntoSpan_ReadOnly(int count)
    {
        ReadOnlySpan<byte> span = _bytes.AsSpan();
        var target = new byte[count * 2];
        var expected = new byte[target.Length];
        _bytes.AsSpan(0, count).CopyTo(expected);

        Assert.That(span.Read(target.AsSpan(0, count), out var val).SequenceEqual(span[count..]));
        target.ShouldBe(expected);
        val.ShouldBe(count);
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Read_IntoMemory(int count)
    {
        var span = _bytes.AsSpan();
        var target = new byte[count * 2];
        var expected = new byte[target.Length];
        _bytes.AsSpan(0, count).CopyTo(expected);

        Assert.That(span.Read(target.AsMemory(0, count), out var val).SequenceEqual(span[count..]));
        target.ShouldBe(expected);
        val.ShouldBe(count);
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Read_IntoMemory_ReadOnly(int count)
    {
        ReadOnlySpan<byte> span = _bytes.AsSpan();
        var target = new byte[count * 2];
        var expected = new byte[target.Length];
        _bytes.AsSpan(0, count).CopyTo(expected);

        Assert.That(span.Read(target.AsMemory(0, count), out var val).SequenceEqual(span[count..]));
        target.ShouldBe(expected);
        val.ShouldBe(count);
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryRead_Array(int count)
    {
        Assert.Multiple(() =>
        {
            var span = _bytes.AsSpan();
            var size = Math.Min(span.Length, count);
            var remainder = span[size..];
            Assert.That(span.TryRead(count, out var val).SequenceEqual(remainder));
            Assert.That(val.AsSpan().SequenceEqual(span[..size]));
        });
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryRead_Array_ReadOnly(int count)
    {
        Assert.Multiple(() =>
        {
            ReadOnlySpan<byte> span = _bytes.AsSpan();
            var size = Math.Min(span.Length, count);
            var remainder = span[size..];
            Assert.That(span.TryRead(count, out var val).SequenceEqual(remainder));
            Assert.That(val.AsSpan().SequenceEqual(span[..size]));
        });
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryRead_IntoSpan(int count)
    {
        var span = _bytes.AsSpan();
        var target = new byte[count * 2];
        var expected = new byte[target.Length];
        var size = Math.Min(span.Length, count);
        _bytes.AsSpan(0, size).CopyTo(expected);

        Assert.That(span.TryRead(target.AsSpan(0, size), out var val).SequenceEqual(span[size..]));
        target.ShouldBe(expected);
        val.ShouldBe(size);
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryRead_IntoSpan_ReadOnly(int count)
    {
        ReadOnlySpan<byte> span = _bytes.AsSpan();
        var target = new byte[count * 2];
        var expected = new byte[target.Length];
        var size = Math.Min(span.Length, count);
        _bytes.AsSpan(0, size).CopyTo(expected);

        Assert.That(span.TryRead(target.AsSpan(0, size), out var val).SequenceEqual(span[size..]));
        target.ShouldBe(expected);
        val.ShouldBe(size);
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryRead_IntoMemory(int count)
    {
        var span = _bytes.AsSpan();
        var target = new byte[count * 2];
        var expected = new byte[target.Length];
        var size = Math.Min(span.Length, count);
        _bytes.AsSpan(0, size).CopyTo(expected);

        Assert.That(span.TryRead(target.AsMemory(0, size), out var val).SequenceEqual(span[size..]));
        target.ShouldBe(expected);
        val.ShouldBe(size);
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryRead_IntoMemory_ReadOnly(int count)
    {
        ReadOnlySpan<byte> span = _bytes.AsSpan();
        var target = new byte[count * 2];
        var expected = new byte[target.Length];
        var size = Math.Min(span.Length, count);
        _bytes.AsSpan(0, size).CopyTo(expected);

        Assert.That(span.TryRead(target.AsMemory(0, size), out var val).SequenceEqual(span[size..]));
        target.ShouldBe(expected);
        val.ShouldBe(size);
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Write_Span(int count)
    {
        var span = _bytes.AsSpan();
        var source = new byte[count];
        var remainder = _bytes.AsSpan(count);
        var expected = _bytes.ToArray();
        source.AsSpan().CopyTo(expected);
        
        Assert.That(span.Write(source.AsSpan(), out var val).SequenceEqual(remainder));
        _bytes.ShouldBe(expected);
        val.ShouldBe(count);
    }
    
    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Write_Memory(int count)
    {
        var span = _bytes.AsSpan();
        var source = new byte[count];
        var remainder = _bytes.AsSpan(count);
        var expected = _bytes.ToArray();
        source.AsSpan().CopyTo(expected);
        
        Assert.That(span.Write(source.AsMemory(), out var val).SequenceEqual(remainder));
        _bytes.ShouldBe(expected);
        val.ShouldBe(count);
    }
    
    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryWrite_Span(int count)
    {
        var span = _bytes.AsSpan();
        var source = new byte[count];
        var expected = _bytes.ToArray();
        var size = Math.Min(count, span.Length);
        var remainder = _bytes.AsSpan(size);
        source.AsSpan(0, size).CopyTo(expected);
        
        Assert.That(span.TryWrite(source.AsSpan(), out var val).SequenceEqual(remainder));
        _bytes.ShouldBe(expected);
        val.ShouldBe(size);
    }
    
    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryWrite_Memory(int count)
    {
        var span = _bytes.AsSpan();
        var source = new byte[count];
        var expected = _bytes.ToArray();
        var size = Math.Min(count, span.Length);
        var remainder = _bytes.AsSpan(size);
        source.AsSpan(0, size).CopyTo(expected);
        
        Assert.That(span.TryWrite(source.AsMemory(), out var val).SequenceEqual(remainder));
        _bytes.ShouldBe(expected);
        val.ShouldBe(size);
    }
}