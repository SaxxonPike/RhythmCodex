using System;
using System.Linq;
using Autofac;
using ClientCommon;
using Shouldly;
using NUnit.Framework;

namespace RhythmCodex.Cli.Modules;

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
        var parsedArgs = AppContainer.Resolve<ArgParser>().Parse([inputFileName]);

        // Act.
        Action act = () => subject
            .Commands
            .Single(c => c.Name.Equals("decode", StringComparison.OrdinalIgnoreCase))
            .TaskFactory(parsedArgs);

        // Assert.
        act.ShouldNotThrow();
    }
}