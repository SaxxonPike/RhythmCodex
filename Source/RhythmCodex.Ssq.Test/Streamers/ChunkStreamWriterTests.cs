using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers
{
    public class ChunkStreamWriterTests : ChunkStreamBaseTests<ChunkStreamWriter>
    {
        [Test]
        public void Write_WritesValidChunk()
        {
            // Arrange.
            var param0 = Create<short>();
            var param1 = Create<short>();
            var data = CreateMany<byte>().ToArray();
            var expected = PrepareChunk(param0, param1, data);
            var stream = new MemoryStream();

            // Act.
            Subject.Write(stream, new Chunk { Parameter0 = param0, Parameter1 = param1, Data = data });
            stream.Flush();

            // Assert.
            var observed = stream.ToArray();
            observed.ShouldAllBeEquivalentTo(expected);
        }

        [Test]
        public void Write_WritesEndChunk()
        {
            // Arrange.
            var stream = new MemoryStream();

            // Act.
            Subject.Write(stream, null);
            stream.Flush();

            // Assert.
            var observed = stream.ToArray();
            observed.ShouldAllBeEquivalentTo(new byte[] { 0, 0, 0, 0 });
        }
    }
}
