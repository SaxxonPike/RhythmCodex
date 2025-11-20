using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using RhythmCodex.Charts.Ssq.Model;
using Shouldly;

namespace RhythmCodex.Charts.Ssq.Streamers;

[TestFixture]
public class SsqStreamerTests : BaseUnitTestFixture<SsqStreamReader, ISsqStreamReader>
{
    [Test]
    public void Read_ReadsAllChunks()
    {
        // Arrange.
        var chunks = CreateMany<SsqChunk>().Concat([null]).ToList();
        var chunkStreamer = Mock<IChunkStreamReader>();
        var chunkIndex = 0;
        var stream = new MemoryStream();
        chunkStreamer.Setup(x => x.Read(It.IsAny<Stream>()))
            .Returns(() => chunks[chunkIndex++]);

        // Act.
        var result = Subject.Read(stream);

        // Assert.
        result.ShouldBe(chunks.TakeWhile(c => c != null));
    }
}