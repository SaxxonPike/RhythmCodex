using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Charts.Ssq.Model;
using Shouldly;

namespace RhythmCodex.Charts.Ssq.Streamers;

public class ChunkStreamWriterTests : ChunkStreamBaseTests<ChunkStreamWriter, IChunkStreamWriter>
{
    [Test]
    public void Write_WritesEndChunk()
    {
        // Arrange.
        var stream = new MemoryStream();

        // Act.
        Subject.WriteEnd(stream);
        stream.Flush();

        // Assert.
        var observed = stream.ToArray();
        observed.ShouldBeEquivalentTo(new byte[] {0, 0, 0, 0});
    }

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
        Subject.Write(stream, new SsqChunk {Parameter0 = param0, Parameter1 = param1, Data = data});
        stream.Flush();

        // Assert.
        var observed = stream.ToArray();
        observed.ShouldBeEquivalentTo(expected);
    }
}