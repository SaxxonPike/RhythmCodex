using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace RhythmCodex.Djmain.Streamers
{
    [TestFixture]
    public class Pcm16StreamReaderTests : BaseUnitTestFixture<AudioStreamReader>
    {
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
