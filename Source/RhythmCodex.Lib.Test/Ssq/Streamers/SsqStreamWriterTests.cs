using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shouldly;
using Moq;
using NUnit.Framework;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers;

[TestFixture]
public class SsqStreamWriterTests : BaseUnitTestFixture<SsqStreamWriter, ISsqStreamWriter>
{
    [Test]
    public void Write_WritesAllChunks()
    {
        // Arrange.
        var chunks = CreateMany<SsqChunk?>().Concat([null]).ToList();
        var chunkStreamer = Mock<IChunkStreamWriter>();
        var stream = new MemoryStream();
        var result = new List<SsqChunk>();
        chunkStreamer.Setup(x => x.Write(It.IsAny<Stream>(), It.IsAny<SsqChunk>()))
            .Callback<Stream, SsqChunk>((_, c) => result.Add(c));

        // Act.
        Subject.Write(stream, chunks);

        // Assert.
        result.ShouldBe(chunks.Concat(new SsqChunk[] {null}));
    }
}