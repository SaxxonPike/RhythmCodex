using System;
using System.IO;
using System.Linq;
using Shouldly;
using NUnit.Framework;
using RhythmCodex.Bms.Converters;
using RhythmCodex.Bms.Model;
using RhythmCodex.Bms.Streamers;

namespace RhythmCodex.Bms.Integration;

[TestFixture]
public class BmsIntegrationTests : BaseIntegrationFixture
{
    [TestCase("positive")]
    public void Test_ReadingExampleBms(string name)
    {
        // Arrange.
        var data = GetArchiveResource($"Bms.{name}.zip")
            .First(f => f.Key.EndsWith(".bms", StringComparison.OrdinalIgnoreCase))
            .Value;
        var mem = new MemoryStream(data);
        var reader = Resolve<IBmsStreamReader>();
        var resolver = Resolve<IBmsRandomResolver>();
        var decoder = Resolve<IBmsDecoder>();

        // Act.
        var commands = reader.Read(mem);
        var resolved = resolver.Resolve(commands);
        var decoded = decoder.Decode(resolved);
    }

    [Test]
    [Explicit]
    public void Test_ReadingSampleMap()
    {
        // Arrange.
        var data = GetArchiveResource($"Bms.random.zip")
            .First()
            .Value;
        var mem = new MemoryStream(data);
        var reader = Resolve<IBmsStreamReader>();
        var resolver = Resolve<IBmsRandomResolver>();
        var decoder = Resolve<IBmsDecoder>();

        // Act.
        var commands = reader.Read(mem);
        var resolved = resolver.Resolve(commands);
        var decoded = decoder.Decode(resolved);
    }

    [Test]
    public void Test_ParsingLargeRandom()
    {
        // Arrange.
        var data = GetArchiveResource($"Bms.random.zip")
            .First()
            .Value;
        var mem = new MemoryStream(data);
        var reader = Resolve<IBmsStreamReader>();
        var resolver = Resolve<IBmsRandomResolver>();

        // Act.
        var observed = reader.Read(mem);
        Should.NotThrow(() => resolver.Resolve(observed));
    }

    [Test]
    public void Test_ParsingOutComments()
    {
        // Arrange.
        var mem = new MemoryStream();
        var text = string.Join(Environment.NewLine,
            " #WAV04 test.wav",
            "#WAV05 /* hey have a comment",
            "and another */ test2.wav",
            "#TITLE and a // third comment",
            "#ARTIST fourth");
        var textWriter = new StreamWriter(mem);
        textWriter.Write(text);
        textWriter.Flush();
        mem.Position = 0;
        var reader = Resolve<IBmsStreamReader>();

        // Act.
        var observed = reader.Read(mem);

        // Assert.
        observed.ShouldBeEquivalentTo(
            new[]
            {
                new BmsCommand { Name = "WAV04", Value = "test.wav" },
                new BmsCommand { Name = "WAV05", Value = "test2.wav" },
                new BmsCommand { Name = "TITLE", Value = "and a" },
                new BmsCommand { Name = "ARTIST", Value = "fourth" }
            }
        );
    }
}