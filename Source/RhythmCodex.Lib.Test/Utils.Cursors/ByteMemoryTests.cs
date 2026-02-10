using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace RhythmCodex.Utils.Cursors;

[Parallelizable(ParallelScope.None)]
public class ByteMemoryTests : BaseTestFixture
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
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadU8(out var observed).Span.SequenceEqual(memory.Span[1..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadU8_ReadOnly()
    {
        var expected = _bytes[0];
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadU8(out var observed).Span.SequenceEqual(memory.Span[1..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU8()
    {
        var val = Random.NextByte();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        expected[0] = val;

        Assert.That(memory.WriteU8(val).Span.SequenceEqual(memory.Span[1..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadS8()
    {
        var expected = unchecked((sbyte)_bytes[0]);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadS8(out var observed).Span.SequenceEqual(memory.Span[1..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadS8_ReadOnly()
    {
        var expected = unchecked((sbyte)_bytes[0]);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadS8(out var observed).Span.SequenceEqual(memory.Span[1..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS8()
    {
        var val = Random.NextSByte();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        expected[0] = unchecked((byte)val);

        Assert.That(memory.WriteS8(val).Span.SequenceEqual(memory.Span[1..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadU16L()
    {
        var expected = ReadUInt16LittleEndian(_bytes);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadU16L(out var observed).Span.SequenceEqual(memory.Span[2..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadU16L_ReadOnly()
    {
        var expected = ReadUInt16LittleEndian(_bytes);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadU16L(out var observed).Span.SequenceEqual(memory.Span[2..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU16L()
    {
        var val = Random.NextUShort();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        WriteUInt16LittleEndian(expected, val);

        Assert.That(memory.WriteU16L(val).Span.SequenceEqual(memory.Span[2..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadU16B()
    {
        var expected = ReadUInt16BigEndian(_bytes);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadU16B(out var observed).Span.SequenceEqual(memory.Span[2..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadU16B_ReadOnly()
    {
        var expected = ReadUInt16BigEndian(_bytes);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadU16B(out var observed).Span.SequenceEqual(memory.Span[2..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU16B()
    {
        var val = Random.NextUShort();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        WriteUInt16BigEndian(expected, val);

        Assert.That(memory.WriteU16B(val).Span.SequenceEqual(memory.Span[2..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadS16L()
    {
        var expected = ReadInt16LittleEndian(_bytes);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadS16L(out var observed).Span.SequenceEqual(memory.Span[2..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadS16L_ReadOnly()
    {
        var expected = ReadInt16LittleEndian(_bytes);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadS16L(out var observed).Span.SequenceEqual(memory.Span[2..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS16L()
    {
        var val = Random.NextShort();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        WriteInt16LittleEndian(expected, val);

        Assert.That(memory.WriteS16L(val).Span.SequenceEqual(memory.Span[2..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadS16B()
    {
        var expected = ReadInt16BigEndian(_bytes);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadS16B(out var observed).Span.SequenceEqual(memory.Span[2..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadS16B_ReadOnly()
    {
        var expected = ReadInt16BigEndian(_bytes);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadS16B(out var observed).Span.SequenceEqual(memory.Span[2..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS16B()
    {
        var val = Random.NextShort();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        WriteInt16BigEndian(expected, val);

        Assert.That(memory.WriteS16B(val).Span.SequenceEqual(memory.Span[2..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadU32L()
    {
        var expected = ReadUInt32LittleEndian(_bytes);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadU32L(out var observed).Span.SequenceEqual(memory.Span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadU32L_ReadOnly()
    {
        var expected = ReadUInt32LittleEndian(_bytes);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadU32L(out var observed).Span.SequenceEqual(memory.Span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU32L()
    {
        var val = Random.NextUShort();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        WriteUInt32LittleEndian(expected, val);

        Assert.That(memory.WriteU32L(val).Span.SequenceEqual(memory.Span[4..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadU32B()
    {
        var expected = ReadUInt32BigEndian(_bytes);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadU32B(out var observed).Span.SequenceEqual(memory.Span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadU32B_ReadOnly()
    {
        var expected = ReadUInt32BigEndian(_bytes);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadU32B(out var observed).Span.SequenceEqual(memory.Span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU32B()
    {
        var val = Random.NextUShort();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        WriteUInt32BigEndian(expected, val);

        Assert.That(memory.WriteU32B(val).Span.SequenceEqual(memory.Span[4..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadS32L()
    {
        var expected = ReadInt32LittleEndian(_bytes);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadS32L(out var observed).Span.SequenceEqual(memory.Span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadS32L_ReadOnly()
    {
        var expected = ReadInt32LittleEndian(_bytes);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadS32L(out var observed).Span.SequenceEqual(memory.Span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS32L()
    {
        var val = Random.NextShort();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        WriteInt32LittleEndian(expected, val);

        Assert.That(memory.WriteS32L(val).Span.SequenceEqual(memory.Span[4..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadS32B()
    {
        var expected = ReadInt32BigEndian(_bytes);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadS32B(out var observed).Span.SequenceEqual(memory.Span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadS32B_ReadOnly()
    {
        var expected = ReadInt32BigEndian(_bytes);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadS32B(out var observed).Span.SequenceEqual(memory.Span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS32B()
    {
        var val = Random.NextShort();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        WriteInt32BigEndian(expected, val);

        Assert.That(memory.WriteS32B(val).Span.SequenceEqual(memory.Span[4..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadF32L()
    {
        var expected = ReadSingleLittleEndian(_bytes);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadF32L(out var observed).Span.SequenceEqual(memory.Span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadF32L_ReadOnly()
    {
        var expected = ReadSingleLittleEndian(_bytes);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadF32L(out var observed).Span.SequenceEqual(memory.Span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteF32L()
    {
        var val = Random.NextShort();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        WriteSingleLittleEndian(expected, val);

        Assert.That(memory.WriteF32L(val).Span.SequenceEqual(memory.Span[4..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadF32B()
    {
        var expected = ReadSingleBigEndian(_bytes);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadF32B(out var observed).Span.SequenceEqual(memory.Span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadF32B_ReadOnly()
    {
        var expected = ReadSingleBigEndian(_bytes);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadF32B(out var observed).Span.SequenceEqual(memory.Span[4..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteF32B()
    {
        var val = Random.NextShort();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        WriteSingleBigEndian(expected, val);

        Assert.That(memory.WriteF32B(val).Span.SequenceEqual(memory.Span[4..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadU64L()
    {
        var expected = ReadUInt64LittleEndian(_bytes);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadU64L(out var observed).Span.SequenceEqual(memory.Span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadU64L_ReadOnly()
    {
        var expected = ReadUInt64LittleEndian(_bytes);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadU64L(out var observed).Span.SequenceEqual(memory.Span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU64L()
    {
        var val = Random.NextUShort();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        WriteUInt64LittleEndian(expected, val);

        Assert.That(memory.WriteU64L(val).Span.SequenceEqual(memory.Span[8..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadU64B()
    {
        var expected = ReadUInt64BigEndian(_bytes);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadU64B(out var observed).Span.SequenceEqual(memory.Span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadU64B_ReadOnly()
    {
        var expected = ReadUInt64BigEndian(_bytes);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadU64B(out var observed).Span.SequenceEqual(memory.Span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU64B()
    {
        var val = Random.NextUShort();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        WriteUInt64BigEndian(expected, val);

        Assert.That(memory.WriteU64B(val).Span.SequenceEqual(memory.Span[8..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadS64L()
    {
        var expected = ReadInt64LittleEndian(_bytes);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadS64L(out var observed).Span.SequenceEqual(memory.Span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadS64L_ReadOnly()
    {
        var expected = ReadInt64LittleEndian(_bytes);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadS64L(out var observed).Span.SequenceEqual(memory.Span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS64L()
    {
        var val = Random.NextShort();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        WriteInt64LittleEndian(expected, val);

        Assert.That(memory.WriteS64L(val).Span.SequenceEqual(memory.Span[8..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadS64B()
    {
        var expected = ReadInt64BigEndian(_bytes);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadS64B(out var observed).Span.SequenceEqual(memory.Span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadS64B_ReadOnly()
    {
        var expected = ReadInt64BigEndian(_bytes);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadS64B(out var observed).Span.SequenceEqual(memory.Span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS64B()
    {
        var val = Random.NextShort();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        WriteInt64BigEndian(expected, val);

        Assert.That(memory.WriteS64B(val).Span.SequenceEqual(memory.Span[8..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadF64L()
    {
        var expected = ReadDoubleLittleEndian(_bytes);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadF64L(out var observed).Span.SequenceEqual(memory.Span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadF64L_ReadOnly()
    {
        var expected = ReadDoubleLittleEndian(_bytes);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadF64L(out var observed).Span.SequenceEqual(memory.Span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteF64L()
    {
        var val = Random.NextShort();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        WriteDoubleLittleEndian(expected, val);

        Assert.That(memory.WriteF64L(val).Span.SequenceEqual(memory.Span[8..]));
        AssertBytes(expected);
    }

    [Test]
    public void Test_ReadF64B()
    {
        var expected = ReadDoubleBigEndian(_bytes);
        var memory = _bytes.AsMemory();

        Assert.That(memory.ReadF64B(out var observed).Span.SequenceEqual(memory.Span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadF64B_ReadOnly()
    {
        var expected = ReadDoubleBigEndian(_bytes);
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();

        Assert.That(memory.ReadF64B(out var observed).Span.SequenceEqual(memory.Span[8..]));
        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteF64B()
    {
        var val = Random.NextShort();
        var memory = _bytes.AsMemory();
        var expected = _bytes.ToArray();
        WriteDoubleBigEndian(expected, val);

        Assert.That(memory.WriteF64B(val).Span.SequenceEqual(memory.Span[8..]));
        AssertBytes(expected);
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Skip(int count)
    {
        var memory = _bytes.AsMemory();
        Assert.That(memory.Skip(count).Span.SequenceEqual(memory.Span[count..]));
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Skip_ReadOnly(int count)
    {
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();
        Assert.That(memory.Skip(count).Span.SequenceEqual(memory.Span[count..]));
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Extract(int count)
    {
        Assert.Multiple(() =>
        {
            var memory = _bytes.AsMemory();
            Assert.That(memory.Extract(count, out var val).Span.SequenceEqual(memory.Span[count..]));
            Assert.That(val.Span.SequenceEqual(memory.Span[..count]));
        });
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Extract_ReadOnly(int count)
    {
        Assert.Multiple(() =>
        {
            ReadOnlyMemory<byte> memory = _bytes.AsMemory();
            Assert.That(memory.Extract(count, out var val).Span.SequenceEqual(memory.Span[count..]));
            Assert.That(val.Span.SequenceEqual(memory.Span[..count]));
        });
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryExtract(int count)
    {
        Assert.Multiple(() =>
        {
            var memory = _bytes.AsMemory();
            var size = Math.Min(count, memory.Length);
            var remainder = memory[size..];
            Assert.That(memory.TryExtract(count, out var val).Span.SequenceEqual(remainder.Span));
            Assert.That(val.Span.SequenceEqual(memory.Span[..size]));
        });
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryExtract_ReadOnly(int count)
    {
        Assert.Multiple(() =>
        {
            ReadOnlyMemory<byte> memory = _bytes.AsMemory();
            var size = Math.Min(count, memory.Length);
            var remainder = memory[Math.Min(count, memory.Length)..];
            Assert.That(memory.TryExtract(count, out var val).Span.SequenceEqual(remainder.Span));
            Assert.That(val.Span.SequenceEqual(memory.Span[..size]));
        });
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Read_Array(int count)
    {
        Assert.Multiple(() =>
        {
            var memory = _bytes.AsMemory();
            var remainder = memory[Math.Min(count, memory.Length)..];
            Assert.That(memory.Read(count, out var val).Span.SequenceEqual(remainder.Span));
            Assert.That(val.AsMemory().Span.SequenceEqual(memory.Span[..count]));
        });
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Read_Array_ReadOnly(int count)
    {
        Assert.Multiple(() =>
        {
            ReadOnlyMemory<byte> memory = _bytes.AsMemory();
            var remainder = memory[Math.Min(count, memory.Length)..];
            Assert.That(memory.Read(count, out var val).Span.SequenceEqual(remainder.Span));
            Assert.That(val.AsMemory().Span.SequenceEqual(memory.Span[..count]));
        });
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Read_IntoSpan(int count)
    {
        var memory = _bytes.AsMemory();
        var target = new byte[count * 2];
        var expected = new byte[target.Length];
        _bytes.AsMemory(0, count).CopyTo(expected);

        Assert.That(memory.Read(target.AsSpan(0, count), out var val).Span.SequenceEqual(memory.Span[count..]));
        target.ShouldBe(expected);
        val.ShouldBe(count);
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Read_IntoSpan_ReadOnly(int count)
    {
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();
        var target = new byte[count * 2];
        var expected = new byte[target.Length];
        _bytes.AsMemory(0, count).CopyTo(expected);

        Assert.That(memory.Read(target.AsSpan(0, count), out var val).Span.SequenceEqual(memory.Span[count..]));
        target.ShouldBe(expected);
        val.ShouldBe(count);
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Read_IntoMemory(int count)
    {
        var memory = _bytes.AsMemory();
        var target = new byte[count * 2];
        var expected = new byte[target.Length];
        _bytes.AsMemory(0, count).CopyTo(expected);

        Assert.That(memory.Read(target.AsMemory(0, count), out var val).Span.SequenceEqual(memory.Span[count..]));
        target.ShouldBe(expected);
        val.ShouldBe(count);
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Read_IntoMemory_ReadOnly(int count)
    {
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();
        var target = new byte[count * 2];
        var expected = new byte[target.Length];
        _bytes.AsMemory(0, count).CopyTo(expected);

        Assert.That(memory.Read(target.AsMemory(0, count), out var val).Span.SequenceEqual(memory.Span[count..]));
        target.ShouldBe(expected);
        val.ShouldBe(count);
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryRead_Array(int count)
    {
        Assert.Multiple(() =>
        {
            var memory = _bytes.AsMemory();
            var size = Math.Min(memory.Length, count);
            var remainder = memory[size..];
            Assert.That(memory.TryRead(count, out var val).Span.SequenceEqual(remainder.Span));
            Assert.That(val.AsMemory().Span.SequenceEqual(memory.Span[..size]));
        });
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryRead_Array_ReadOnly(int count)
    {
        Assert.Multiple(() =>
        {
            ReadOnlyMemory<byte> memory = _bytes.AsMemory();
            var size = Math.Min(memory.Length, count);
            var remainder = memory[size..];
            Assert.That(memory.TryRead(count, out var val).Span.SequenceEqual(remainder.Span));
            Assert.That(val.AsMemory().Span.SequenceEqual(memory.Span[..size]));
        });
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryRead_IntoMemory(int count)
    {
        var memory = _bytes.AsMemory();
        var target = new byte[count * 2];
        var expected = new byte[target.Length];
        var size = Math.Min(memory.Length, count);
        _bytes.AsMemory(0, size).CopyTo(expected);

        Assert.That(memory.TryRead(target.AsMemory(0, size), out var val).Span.SequenceEqual(memory.Span[size..]));
        target.ShouldBe(expected);
        val.ShouldBe(size);
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryRead_IntoMemory_ReadOnly(int count)
    {
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();
        var target = new byte[count * 2];
        var expected = new byte[target.Length];
        var size = Math.Min(memory.Length, count);
        _bytes.AsMemory(0, size).CopyTo(expected);

        Assert.That(memory.TryRead(target.AsMemory(0, size), out var val).Span.SequenceEqual(memory.Span[size..]));
        target.ShouldBe(expected);
        val.ShouldBe(size);
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryRead_IntoSpan(int count)
    {
        var memory = _bytes.AsMemory();
        var target = new byte[count * 2];
        var expected = new byte[target.Length];
        var size = Math.Min(memory.Length, count);
        _bytes.AsMemory(0, size).CopyTo(expected);

        Assert.That(memory.TryRead(target.AsSpan(0, size), out var val).Span.SequenceEqual(memory.Span[size..]));
        target.ShouldBe(expected);
        val.ShouldBe(size);
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryRead_IntoSpan_ReadOnly(int count)
    {
        ReadOnlyMemory<byte> memory = _bytes.AsMemory();
        var target = new byte[count * 2];
        var expected = new byte[target.Length];
        var size = Math.Min(memory.Length, count);
        _bytes.AsMemory(0, size).CopyTo(expected);

        Assert.That(memory.TryRead(target.AsSpan(0, size), out var val).Span.SequenceEqual(memory.Span[size..]));
        target.ShouldBe(expected);
        val.ShouldBe(size);
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Write_Span(int count)
    {
        var memory = _bytes.AsMemory();
        var source = new byte[count];
        var remainder = _bytes.AsMemory(count);
        var expected = _bytes.ToArray();
        source.AsMemory().CopyTo(expected);

        Assert.That(memory.Write(source.AsSpan(), out var val).Span.SequenceEqual(remainder.Span));
        _bytes.ShouldBe(expected);
        val.ShouldBe(count);
    }

    [Test]
    [TestCaseSource(nameof(WithinBoundsCounts))]
    public void Test_Write_Memory(int count)
    {
        var memory = _bytes.AsMemory();
        var source = new byte[count];
        var remainder = _bytes.AsMemory(count);
        var expected = _bytes.ToArray();
        source.AsMemory().CopyTo(expected);

        Assert.That(memory.Write(source.AsMemory(), out var val).Span.SequenceEqual(remainder.Span));
        _bytes.ShouldBe(expected);
        val.ShouldBe(count);
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryWrite_Span(int count)
    {
        var memory = _bytes.AsMemory();
        var source = new byte[count];
        var expected = _bytes.ToArray();
        var size = Math.Min(count, memory.Length);
        var remainder = _bytes.AsMemory(size);
        source.AsMemory(0, size).CopyTo(expected);

        Assert.That(memory.TryWrite(source.AsSpan(), out var val).Span.SequenceEqual(remainder.Span));
        _bytes.ShouldBe(expected);
        val.ShouldBe(size);
    }

    [Test]
    [TestCaseSource(nameof(OutOfBoundsCounts))]
    public void Test_TryWrite_Memory(int count)
    {
        var memory = _bytes.AsMemory();
        var source = new byte[count];
        var expected = _bytes.ToArray();
        var size = Math.Min(count, memory.Length);
        var remainder = _bytes.AsMemory(size);
        source.AsMemory(0, size).CopyTo(expected);

        Assert.That(memory.TryWrite(source.AsMemory(), out var val).Span.SequenceEqual(remainder.Span));
        _bytes.ShouldBe(expected);
        val.ShouldBe(size);
    }
}