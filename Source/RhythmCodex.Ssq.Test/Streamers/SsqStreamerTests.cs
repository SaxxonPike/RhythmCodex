﻿using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RhythmCodex.Ssq.Model;
using RhythmCodex.Streamers;

namespace RhythmCodex.Ssq.Streamers
{
    [TestFixture]
    public class SsqStreamerTests : BaseTestFixture<SsqStreamReader>
    {
        [Test]
        public void Read_ReadsAllChunks()
        {
            // Arrange.
            var chunks = Fixture.CreateMany<Chunk>().Cast<Chunk?>().Concat(new Chunk?[] {null}).ToList();
            var chunkStreamer = Mock<IStreamReader<Chunk?>>();
            var chunkIndex = 0;
            var stream = new MemoryStream();
            chunkStreamer.Setup(x => x.Read(It.IsAny<Stream>()))
                .Returns(() => chunks[chunkIndex++]);

            // Act.
            var result = Subject.Read(stream);

            // Assert.
            result.ShouldAllBeEquivalentTo(chunks.TakeWhile(c => c != null));
        }
    }
}
