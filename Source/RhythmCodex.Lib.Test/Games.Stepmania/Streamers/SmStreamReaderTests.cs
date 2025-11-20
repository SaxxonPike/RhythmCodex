using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Charts.Sm.Model;
using RhythmCodex.Charts.Sm.Streamers;
using Shouldly;

namespace RhythmCodex.Games.Stepmania.Streamers;

[TestFixture]
public class SmStreamReaderTests : BaseUnitTestFixture<SmStreamReader, ISmStreamReader>
{
    [Test]
    public void Read_ReadsCommands()
    {
        using var mem = new MemoryStream();
        // Arrange.
        var expected = new[]
        {
            new Command { Name = Create<string>(), Values = CreateMany<string>().ToList() },
            new Command { Name = Create<string>(), Values = CreateMany<string>().ToList() },
            new Command { Name = Create<string>(), Values = CreateMany<string>().ToList() }
        };

        var input = string.Join(Environment.NewLine,
            expected.Select(c => $"#{c.Name}:{string.Join(":", c.Values)};"));
        var writer = new StreamWriter(mem);
        writer.Write(input);
        writer.Flush();
        mem.Position = 0;

        // Act.
        var result = Subject.Read(mem).ToArray();

        // Assert.
        result.ShouldBeEquivalentTo(expected);
    }

    [Test]
    public void Read_ReadsMultilineCommand()
    {
        using var mem = new MemoryStream();
        // Arrange.
        var expected = new[]
        {
            new Command
            {
                Name = Create<string>(),
                Values = [string.Join(Environment.NewLine, CreateMany<string>())]
            },
            new Command
            {
                Name = Create<string>(),
                Values = [string.Join(Environment.NewLine, CreateMany<string>())]
            }
        };

        var input = string.Join(Environment.NewLine,
            expected.Select(c => $"#{c.Name}:{string.Join(":", c.Values)};"));
        var writer = new StreamWriter(mem);
        writer.Write(input);
        writer.Flush();
        mem.Position = 0;

        // Act.
        var result = Subject.Read(mem).ToArray();

        // Assert.
        result.ShouldBeEquivalentTo(expected);
    }
}