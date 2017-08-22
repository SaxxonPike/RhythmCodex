using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace RhythmCodex.Djmain.Streamers
{
    [TestFixture]
    public class DpcmStreamReaderTests : BaseUnitTestFixture<AudioStreamReader>
    {
        [Test]
        public void Read_EndsImmediatelyWithNoData()
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
        public void Read_EndsImmediatelyOnImmediateEndMarker()
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
        public void Read_ReadsUntilEndOfStream()
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
        public void Read_ReadsUntilEndMarker()
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
        public void Read_ReadsUntilPartialEndMarker()
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
    }
}
