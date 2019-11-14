using System;
using System.Linq;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Cli.Helpers;

namespace RhythmCodex.Cli.Modules
{
    [TestFixture]
    public class SsqCliModuleTests : AppIntegrationFixture
    {
        [Test]
        [TestCase("shock")]
        [TestCase("freeze")]
        [TestCase("solo")]
        public void Decode_DoesNotThrowOnValidData(string name)
        {
            // Arrange.
            var archiveFileName = $"Ssq.{name}.zip";
            var inputFileName = $"{name}.ssq";

            var inputFile = GetArchiveResource(archiveFileName).Single().Value;
            FileSystem.WriteAllBytes(inputFileName, inputFile);

            var subject = AppContainer.Resolve<SsqCliModule>();
            var parsedArgs = AppContainer.Resolve<ArgParser>().Parse(new[] {inputFileName});

            // Act.
            Action act = () => subject
                .Commands
                .Single(c => c.Name.Equals("decode", StringComparison.OrdinalIgnoreCase))
                .Execute(parsedArgs);

            // Assert.
            act.Should().NotThrow();
        }
    }
}