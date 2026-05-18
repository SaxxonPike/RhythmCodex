using System.IO;
using NUnit.Framework;
using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Riff.Models;
using Shouldly;

namespace RhythmCodex.Sounds.Riff.Streamers;

[TestFixture]
public class RiffStreamWriterTests : BaseUnitTestFixture<RiffStreamWriter, IRiffStreamWriter>
{
    [Test]
    public void Write_WritesRiffContainer()
    {
        using var stream = new MemoryStream();
        var container = new RiffContainer
        {
            Format = "WAVE",
            Chunks =
            [
                new RiffChunk
                {
                    Id = "fmt ",
                    Data = new byte[] { 0x01, 0x00, 0x02, 0x00 }
                },
                new RiffChunk
                {
                    Id = "data",
                    Data = new byte[] { 0x10, 0x20, 0x30 }
                }
            ]
        };

        var result = Subject.Write(stream, container);

        result.ShouldBe(35);
        stream.ToArray().ShouldBe([
            0x52, 0x49, 0x46, 0x46, // RIFF
            0x1B, 0x00, 0x00, 0x00, // RIFF chunk size: 27
            0x57, 0x41, 0x56, 0x45, // WAVE
            0x66, 0x6D, 0x74, 0x20, // fmt 
            0x04, 0x00, 0x00, 0x00, // fmt chunk size: 4
            0x01, 0x00, 0x02, 0x00, // fmt chunk data
            0x64, 0x61, 0x74, 0x61, // data
            0x03, 0x00, 0x00, 0x00, // data chunk size: 3
            0x10, 0x20, 0x30 // data chunk data
        ]);
    }

    [Test]
    public void Write_WithEmptyChunks_WritesRiffHeaderOnly()
    {
        using var stream = new MemoryStream();
        var container = new RiffContainer
        {
            Format = "WAVE",
            Chunks = []
        };

        var result = Subject.Write(stream, container);

        result.ShouldBe(12);
        stream.ToArray().ShouldBe([
            0x52, 0x49, 0x46, 0x46, // RIFF
            0x04, 0x00, 0x00, 0x00, // RIFF chunk size: 4
            0x57, 0x41, 0x56, 0x45 // WAVE
        ]);
    }

    [Test]
    public void Write_WithNullFormat_Throws()
    {
        using var stream = new MemoryStream();
        var container = new RiffContainer
        {
            Format = null,
            Chunks = []
        };

        Should.Throw<RhythmCodexException>(() => Subject.Write(stream, container))
            .Message.ShouldBe("Format string cannot be null.");
    }

    [Test]
    public void Write_WithFormatThatIsNotFourBytes_Throws()
    {
        using var stream = new MemoryStream();
        var container = new RiffContainer
        {
            Format = "WAV",
            Chunks = []
        };

        Should.Throw<RhythmCodexException>(() => Subject.Write(stream, container))
            .Message.ShouldBe("Format string must be 4 bytes. Found: 'WAV'");
    }

    [Test]
    public void Write_WithNullChunkId_Throws()
    {
        using var stream = new MemoryStream();
        var container = new RiffContainer
        {
            Format = "WAVE",
            Chunks =
            [
                new RiffChunk
                {
                    Id = null,
                    Data = new byte[] { 0x01 }
                }
            ]
        };

        Should.Throw<RhythmCodexException>(() => Subject.Write(stream, container))
            .Message.ShouldBe("Chunk ID string cannot be null.");
    }

    [Test]
    public void Write_WithChunkIdThatIsNotFourBytes_Throws()
    {
        using var stream = new MemoryStream();
        var container = new RiffContainer
        {
            Format = "WAVE",
            Chunks =
            [
                new RiffChunk
                {
                    Id = "dat",
                    Data = new byte[] { 0x01 }
                }
            ]
        };

        Should.Throw<RhythmCodexException>(() => Subject.Write(stream, container))
            .Message.ShouldBe("Chunk ID string must be 4 bytes. Found: 'dat'");
    }

    [Test]
    public void Write_WhenValidationFails_DoesNotWriteToStream()
    {
        using var stream = new MemoryStream();
        var container = new RiffContainer
        {
            Format = "WAVE",
            Chunks =
            [
                new RiffChunk
                {
                    Id = "ok  ",
                    Data = new byte[] { 0x01 }
                },
                new RiffChunk
                {
                    Id = "bad",
                    Data = new byte[] { 0x02 }
                }
            ]
        };

        Should.Throw<RhythmCodexException>(() => Subject.Write(stream, container));

        stream.ToArray().ShouldBeEmpty();
    }
}