using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace RhythmCodex.Ssq.Streamers
{
    [TestFixture]
    public class ChunkStreamReaderTests : ChunkStreamBaseTests<ChunkStreamReader, IChunkStreamReader>
    {
        [Test]
        public void Read_ReadsEndChunk()
        {
            // Arrange.
            var stream = new MemoryStream(new byte[] {0, 0, 0, 0});

            // Act.
            var result = Subject.Read(stream);

            // Assert.
            result.Should().BeNull();
        }

        [Test]
        public void Read_ReadsValidChunk()
        {
            // Arrange.
            var param0 = Create<short>();
            var param1 = Create<short>();
            var data = CreateMany<byte>().ToArray();
            var stream = new MemoryStream(PrepareChunk(param0, param1, data));

            // Act.
            var result = Subject.Read(stream);

            // Assert.
            result.Should().NotBe(null);
            result.Parameter0.Should().Be(param0);
            result.Parameter1.Should().Be(param1);
            result.Data.Take(data.Length).Should().BeEquivalentTo(data);
        }
    }
}