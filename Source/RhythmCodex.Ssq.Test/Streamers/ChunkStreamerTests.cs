using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace RhythmCodex.Ssq.Streamers
{
    [TestFixture]
    public class ChunkStreamerTests : ChunkStreamBaseTests<ChunkStreamReader>
    {
        [Test]
        public void Read_ReadsValidChunk()
        {
            // Arrange.
            var param0 = Fixture.Create<short>();
            var param1 = Fixture.Create<short>();
            var data = Fixture.CreateMany<byte>().ToArray();
            var stream = new MemoryStream(PrepareChunk(param0, param1, data));
            
            // Act.
            var result = Subject.Read(stream);

            // Assert.
            result.Should().NotBe(null);
            result.Value.Parameter0.Should().Be(param0);
            result.Value.Parameter1.Should().Be(param1);
            result.Value.Data.ShouldAllBeEquivalentTo(data);
        }

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
    }
}
