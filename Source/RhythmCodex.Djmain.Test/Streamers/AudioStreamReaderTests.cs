using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace RhythmCodex.Djmain.Streamers
{
    [TestFixture]
    public class AudioStreamReaderTests : BaseUnitTestFixture<AudioStreamReader>
    {
        [Test]
        public void ReadDpcm_EndsImmediatelyWithNoData()
        {
            // Arrange.
            var data = new byte[] { };
            var stream = new MemoryStream(data);

            // Act.
            var result = Subject.ReadDpcm(stream);

            // Assert.
            result.Should().BeEmpty();
        }

        [Test]
        public void ReadDpcm_EndsImmediatelyOnImmediateEndMarker()
        {
            // Arrange.
            var data = new byte[] { 0x88, 0x88, 0x88, 0x88 };
            var stream = new MemoryStream(data);

            // Act.
            var result = Subject.ReadDpcm(stream);

            // Assert.
            result.Should().BeEmpty();
        }

        [Test]
        public void ReadDpcm_ReadsUntilEndOfStream()
        {
            // Arrange.
            var expected = new byte[] { 0x12, 0x34, 0x56, 0x78 };
            var stream = new MemoryStream(expected);

            // Act.
            var result = Subject.ReadDpcm(stream);

            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
        }

        [Test]
        public void ReadDpcm_ReadsUntilEndMarker()
        {
            // Arrange.
            var expected = new byte[] { 0x12, 0x34, 0x56, 0x78 };
            var data = expected.Concat(new byte[] { 0x88, 0x88, 0x88, 0x88 }).ToArray();
            var stream = new MemoryStream(data);

            // Act.
            var result = Subject.ReadDpcm(stream);

            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
        }

        [Test]
        public void ReadDpcm_ReadsUntilPartialEndMarker()
        {
            // Arrange.
            var expected = new byte[] { 0x12, 0x34, 0x56, 0x78 };
            var data = expected.Concat(new byte[] { 0x88, 0x88 }).ToArray();
            var stream = new MemoryStream(data);

            // Act.
            var result = Subject.ReadDpcm(stream);

            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
        }

        [Test]
        public void ReadPcm8_EndsImmediatelyWithNoData()
        {
            // Arrange.
            var data = new byte[] { };
            var stream = new MemoryStream(data);

            // Act.
            var result = Subject.ReadPcm8(stream);

            // Assert.
            result.Should().BeEmpty();
        }

        [Test]
        public void ReadPcm8_EndsImmediatelyOnImmediateEndMarker()
        {
            // Arrange.
            var data = new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 };
            var stream = new MemoryStream(data);

            // Act.
            var result = Subject.ReadPcm8(stream);

            // Assert.
            result.Should().BeEmpty();
        }

        [Test]
        public void ReadPcm8_ReadsUntilEndOfStream()
        {
            // Arrange.
            var expected = new byte[] { 0x12, 0x34, 0x56, 0x78 };
            var stream = new MemoryStream(expected);

            // Act.
            var result = Subject.ReadPcm8(stream);

            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
        }

        [Test]
        public void ReadPcm8_ReadsUntilEndMarker()
        {
            // Arrange.
            var expected = new byte[] { 0x12, 0x34, 0x56, 0x78 };
            var data = expected.Concat(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 }).ToArray();
            var stream = new MemoryStream(data);

            // Act.
            var result = Subject.ReadPcm8(stream);

            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
        }

        [Test]
        public void ReadPcm8_ReadsUntilPartialEndMarker()
        {
            // Arrange.
            var expected = new byte[] { 0x12, 0x34, 0x56, 0x78 };
            var data = expected.Concat(new byte[] { 0x80, 0x80, 0x80, 0x80 }).ToArray();
            var stream = new MemoryStream(data);

            // Act.
            var result = Subject.ReadPcm8(stream);

            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
        }

        [Test]
        public void ReadPcm16_EndsImmediatelyWithNoData()
        {
            // Arrange.
            var data = new byte[] { };
            var stream = new MemoryStream(data);

            // Act.
            var result = Subject.ReadPcm16(stream);

            // Assert.
            result.Should().BeEmpty();
        }

        [Test]
        public void ReadPcm16_EndsImmediatelyOnImmediateEndMarker()
        {
            // Arrange.
            var data = new byte[]
            {
                0x00, 0x80, 0x00, 0x80,
                0x00, 0x80, 0x00, 0x80,
                0x00, 0x80, 0x00, 0x80,
                0x00, 0x80, 0x00, 0x80
            };
            var stream = new MemoryStream(data);

            // Act.
            var result = Subject.ReadPcm16(stream);

            // Assert.
            result.Should().BeEmpty();
        }

        [Test]
        public void ReadPcm16_ReadsUntilEndOfStream()
        {
            // Arrange.
            var expected = new byte[] { 0x12, 0x34, 0x56, 0x78 };
            var stream = new MemoryStream(expected);

            // Act.
            var result = Subject.ReadPcm16(stream);

            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
        }

        [Test]
        public void ReadPcm16_ReadsUntilEndMarker()
        {
            // Arrange.
            var expected = new byte[] { 0x12, 0x34, 0x56, 0x78 };
            var data = expected.Concat(new byte[]
            {
                0x00, 0x80, 0x00, 0x80,
                0x00, 0x80, 0x00, 0x80,
                0x00, 0x80, 0x00, 0x80,
                0x00, 0x80, 0x00, 0x80
            }).ToArray();
            var stream = new MemoryStream(data);

            // Act.
            var result = Subject.ReadPcm16(stream);

            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
        }

        [Test]
        public void ReadPcm16_ReadsUntilPartialEndMarker()
        {
            // Arrange.
            var expected = new byte[] { 0x12, 0x34, 0x56, 0x78 };
            var data = expected.Concat(new byte[] { 0x00, 0x80, 0x00, 0x80 }).ToArray();
            var stream = new MemoryStream(data);

            // Act.
            var result = Subject.ReadPcm16(stream);

            // Assert.
            result.ShouldAllBeEquivalentTo(expected);
        }
    }
}
