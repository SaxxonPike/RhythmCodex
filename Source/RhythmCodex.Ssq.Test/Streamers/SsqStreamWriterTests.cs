using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers
{
    [TestFixture]
    public class SsqStreamWriterTests : BaseUnitTestFixture<SsqStreamWriter>
    {
        [Test]
        public void Write_WritesAllChunks()
        {
            // Arrange.
            var chunks = CreateMany<Chunk>().Cast<Chunk?>().Concat(new Chunk?[] { null }).ToList();
            var chunkStreamer = Mock<IChunkStreamWriter>();
            var stream = new MemoryStream();
            var result = new List<Chunk?>();
            chunkStreamer.Setup(x => x.Write(It.IsAny<Stream>(), It.IsAny<Chunk?>()))
                .Callback<Stream, Chunk?>((s, c) => result.Add(c));

            // Act.
            Subject.Write(stream, chunks);

            // Assert.
            result.ShouldAllBeEquivalentTo(chunks.Concat(new Chunk?[] { null }));
        }
    }
}
