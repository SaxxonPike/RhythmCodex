using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers;

[TestFixture]
public class SsqStreamerTests : BaseUnitTestFixture<SsqStreamReader, ISsqStreamReader>
{
    [Test]
    public void Read_ReadsAllChunks()
    {
        // Arrange.
        var chunks = CreateMany<SsqChunk>().Concat(new SsqChunk[] {null}).ToList();
        var chunkStreamer = Mock<IChunkStreamReader>();
        var chunkIndex = 0;
        var stream = new MemoryStream();
        chunkStreamer.Setup(x => x.Read(It.IsAny<Stream>()))
            .Returns(() => chunks[chunkIndex++]);

        // Act.
        var result = Subject.Read(stream);

        // Assert.
        result.Should().BeEquivalentTo(chunks.TakeWhile(c => c != null));
    }
}