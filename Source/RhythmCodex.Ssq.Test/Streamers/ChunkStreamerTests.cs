using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers
{
    [TestFixture]
    public class ChunkStreamerTests : BaseTestFixture<ChunkStreamer>
    {
        private static byte[] PrepareChunk(short param0, short param1, byte[] data)
        {
            return PrepareChunk(data.Length + 8, param0, param1, data);
        }
        
        private static byte[] PrepareChunk(int totalLength, short param0, short param1, byte[] data)
        {
            using (var mem = new MemoryStream())
            using (var writer = new BinaryWriter(mem))
            {
                writer.Write(totalLength);
                writer.Write(param0);
                writer.Write(param1);
                writer.Write(data);
                writer.Flush();
                return mem.ToArray();
            }
        }

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
            result.Parameter0.Should().Be(param0);
            result.Parameter1.Should().Be(param1);
            result.Data.ShouldAllBeEquivalentTo(data);
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

        [Test]
        public void Write_WritesValidChunk()
        {
            // Arrange.
            var param0 = Fixture.Create<short>();
            var param1 = Fixture.Create<short>();
            var data = Fixture.CreateMany<byte>().ToArray();
            var expected = PrepareChunk(param0, param1, data);
            var stream = new MemoryStream();

            // Act.
            Subject.Write(stream, new Chunk {Parameter0 = param0, Parameter1 = param1, Data = data});
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
            observed.ShouldAllBeEquivalentTo(new byte[] {0, 0, 0, 0});
        }
    }
}
