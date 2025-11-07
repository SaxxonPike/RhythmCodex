using System.IO;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace RhythmCodex.Charts.Ssq.Streamers;

[TestFixture]
public class ChunkStreamReaderTests : ChunkStreamBaseTests<ChunkStreamReader, IChunkStreamReader>
{
    [Test]
    public void Read_ReadsEndChunk()
    {
        // Arrange.
        var stream = new MemoryStream([0, 0, 0, 0]);

        // Act.
        var result = Subject.Read(stream);

        // Assert.
        result.ShouldBeNull();
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
        result.ShouldNotBe(null);
        result.Parameter0.ShouldBe(param0);
        result.Parameter1.ShouldBe(param1);
        result.Data.Span[..data.Length].ToArray().ShouldBe(data);
    }
}