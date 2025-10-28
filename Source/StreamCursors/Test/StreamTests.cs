using System.Buffers.Binary;
using Shouldly;

namespace Saxxon.StreamCursors;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class StreamTests : TestFixtureBase
{
    private byte[] _bytes;
    private Stream _stream;

    private class UnseekableStream(byte[] data)
        : MemoryStream(data)
    {
        public override bool CanSeek => false;
    }

    [SetUp]
    public void SetUp()
    {
        var bytes = new byte[16];
        Random.NextBytes(bytes);
        _bytes = bytes;
        _stream = new MemoryStream(_bytes);
    }

    private void AssertStream(byte[] expected)
    {
        var observed = new byte[_bytes.Length];
        _stream.Position = 0;
        _stream.Read(observed)
            .ShouldBe(_bytes.Length);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_Read_FromBeginning()
    {
        var expected = _bytes.AsSpan()[..8].ToArray();

        var observed = _stream.Read(8).ToArray();

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_Read_PastEnd()
    {
        _stream.Position = 8;

        var expected = _bytes.AsSpan()[8..].ToArray();

        var observed = _stream.Read(16).ToArray();

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_ReadU8()
    {
        var expected = _bytes[0];

        _stream.ReadU8(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU8()
    {
        var val = Random.NextByte();
        var expected = _bytes.ToArray();
        expected[0] = val;

        _stream.WriteU8(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadS8()
    {
        var expected = unchecked((sbyte)_bytes[0]);

        _stream.ReadS8(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS8()
    {
        var val = Random.NextSByte();
        var expected = _bytes.ToArray();
        expected[0] = unchecked((byte)val);

        _stream.WriteS8(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadU16L()
    {
        var expected = BinaryPrimitives.ReadUInt16LittleEndian(_bytes);

        _stream.ReadU16L(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU16L()
    {
        var val = Random.NextUShort();
        var expected = _bytes.ToArray();
        BinaryPrimitives.WriteUInt16LittleEndian(expected, val);

        _stream.WriteU16L(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadU16B()
    {
        var expected = BinaryPrimitives.ReadUInt16BigEndian(_bytes);

        _stream.ReadU16B(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU16B()
    {
        var val = Random.NextUShort();
        var expected = _bytes.ToArray();
        BinaryPrimitives.WriteUInt16BigEndian(expected, val);

        _stream.WriteU16B(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadS16L()
    {
        var expected = BinaryPrimitives.ReadInt16LittleEndian(_bytes);

        _stream.ReadS16L(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS16L()
    {
        var val = Random.NextShort();
        var expected = _bytes.ToArray();
        BinaryPrimitives.WriteInt16LittleEndian(expected, val);

        _stream.WriteS16L(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadS16B()
    {
        var expected = BinaryPrimitives.ReadInt16BigEndian(_bytes);

        _stream.ReadS16B(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS16B()
    {
        var val = Random.NextShort();
        var expected = _bytes.ToArray();
        BinaryPrimitives.WriteInt16BigEndian(expected, val);

        _stream.WriteS16B(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadU32L()
    {
        var expected = BinaryPrimitives.ReadUInt32LittleEndian(_bytes);

        _stream.ReadU32L(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU32L()
    {
        var val = Random.NextUInt();
        var expected = _bytes.ToArray();
        BinaryPrimitives.WriteUInt32LittleEndian(expected, val);

        _stream.WriteU32L(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadU32B()
    {
        var expected = BinaryPrimitives.ReadUInt32BigEndian(_bytes);

        _stream.ReadU32B(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU32B()
    {
        var val = Random.NextUInt();
        var expected = _bytes.ToArray();
        BinaryPrimitives.WriteUInt32BigEndian(expected, val);

        _stream.WriteU32B(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadS32L()
    {
        var expected = BinaryPrimitives.ReadInt32LittleEndian(_bytes);

        _stream.ReadS32L(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS32L()
    {
        var val = Random.Next();
        var expected = _bytes.ToArray();
        BinaryPrimitives.WriteInt32LittleEndian(expected, val);

        _stream.WriteS32L(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadS32B()
    {
        var expected = BinaryPrimitives.ReadInt32BigEndian(_bytes);

        _stream.ReadS32B(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS32B()
    {
        var val = Random.Next();
        var expected = _bytes.ToArray();
        BinaryPrimitives.WriteInt32BigEndian(expected, val);

        _stream.WriteS32B(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadF32L()
    {
        var expected = BinaryPrimitives.ReadSingleLittleEndian(_bytes);

        _stream.ReadF32L(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteF32L()
    {
        var val = Random.Next();
        var expected = _bytes.ToArray();
        BinaryPrimitives.WriteSingleLittleEndian(expected, val);

        _stream.WriteF32L(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadF32B()
    {
        var expected = BinaryPrimitives.ReadSingleBigEndian(_bytes);

        _stream.ReadF32B(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteF32B()
    {
        var val = Random.Next();
        var expected = _bytes.ToArray();
        BinaryPrimitives.WriteSingleBigEndian(expected, val);

        _stream.WriteF32B(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadU64L()
    {
        var expected = BinaryPrimitives.ReadUInt64LittleEndian(_bytes);

        _stream.ReadU64L(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU64L()
    {
        var val = Random.NextULong();
        var expected = _bytes.ToArray();
        BinaryPrimitives.WriteUInt64LittleEndian(expected, val);

        _stream.WriteU64L(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadU64B()
    {
        var expected = BinaryPrimitives.ReadUInt64BigEndian(_bytes);

        _stream.ReadU64B(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteU64B()
    {
        var val = Random.NextULong();
        var expected = _bytes.ToArray();
        BinaryPrimitives.WriteUInt64BigEndian(expected, val);

        _stream.WriteU64B(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadS64L()
    {
        var expected = BinaryPrimitives.ReadInt64LittleEndian(_bytes);

        _stream.ReadS64L(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS64L()
    {
        var val = Random.NextLong();
        var expected = _bytes.ToArray();
        BinaryPrimitives.WriteInt64LittleEndian(expected, val);

        _stream.WriteS64L(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadS64B()
    {
        var expected = BinaryPrimitives.ReadInt64BigEndian(_bytes);

        _stream.ReadS64B(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteS64B()
    {
        var val = Random.NextLong();
        var expected = _bytes.ToArray();
        BinaryPrimitives.WriteInt64BigEndian(expected, val);

        _stream.WriteS64B(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadF64L()
    {
        var expected = BinaryPrimitives.ReadDoubleLittleEndian(_bytes);

        _stream.ReadF64L(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteF64L()
    {
        var val = Random.Next();
        var expected = _bytes.ToArray();
        BinaryPrimitives.WriteDoubleLittleEndian(expected, val);

        _stream.WriteF64L(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    public void Test_ReadF64B()
    {
        var expected = BinaryPrimitives.ReadDoubleBigEndian(_bytes);

        _stream.ReadF64B(out var observed)
            .ShouldBeSameAs(_stream);

        observed.ShouldBe(expected);
    }

    [Test]
    public void Test_WriteF64B()
    {
        var val = Random.Next();
        var expected = _bytes.ToArray();
        BinaryPrimitives.WriteDoubleBigEndian(expected, val);

        _stream.WriteF64B(val)
            .ShouldBeSameAs(_stream);

        AssertStream(expected);
    }

    [Test]
    [TestCase(0)]
    [TestCase(4)]
    public void Test_Skip_FromBeginning_Seekable(int count)
    {
        _stream.Skip(count)
            .ShouldBeSameAs(_stream);

        _stream.Position.ShouldBe(count);
    }

    [Test]
    [TestCase(256)]
    public void Test_Skip_PastEnd_Seekable(int count)
    {
        _stream.Skip(count)
            .ShouldBeSameAs(_stream);

        var expected = Math.Min(count, _stream.Position);

        _stream.Position.ShouldBe(expected);
    }

    [Test]
    [TestCase(0)]
    [TestCase(4)]
    public void Test_Skip_FromBeginning_NonSeekable(int count)
    {
        var stream = new UnseekableStream(_bytes);

        stream.Skip(count)
            .ShouldBeSameAs(stream);

        stream.Position.ShouldBe(count);
    }

    [Test]
    [TestCase(256)]
    [TestCase(512)]
    public void Test_Skip_PastEnd_NonSeekable(int count)
    {
        var stream = new UnseekableStream(_bytes);

        stream.Skip(count)
            .ShouldBeSameAs(stream);

        var expected = Math.Min(count, stream.Position);

        stream.Position.ShouldBe(expected);
    }
}