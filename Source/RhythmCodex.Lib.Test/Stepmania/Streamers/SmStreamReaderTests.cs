using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Streamers
{
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
                new Command {Name = Create<string>(), Values = CreateMany<string>().ToArray()},
                new Command {Name = Create<string>(), Values = CreateMany<string>().ToArray()},
                new Command {Name = Create<string>(), Values = CreateMany<string>().ToArray()}
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
            result.Should().BeEquivalentTo(expected);
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
                    Values = new[] {string.Join(Environment.NewLine, CreateMany<string>())}
                },
                new Command
                {
                    Name = Create<string>(),
                    Values = new[] {string.Join(Environment.NewLine, CreateMany<string>())}
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
            result.Should().BeEquivalentTo(expected);
        }
    }
}