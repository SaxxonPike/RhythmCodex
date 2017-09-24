using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using FluentAssertions;
using NUnit.Framework;

namespace RhythmCodex.Cli.Modules
{
    [TestFixture]
    public class SsqCliModuleTests : AppIntegrationFixture
    {
        [Test]
        public void Decode_DoesNotThrowOnValidData(string name)
        {
            // Arrange.
            var archiveFileName = $"Ssq.{name}.zip";
            var inputFileName = $"{name}.ssq";

            var inputFile = GetArchiveResource(archiveFileName).Single().Value;
            FileSystem.WriteAllBytes(inputFileName, inputFile);

            var subject = AppContainer.Resolve<SsqCliModule>();
            var args = new Dictionary<string, string[]>
            {
                {string.Empty, new[] {inputFileName}}
            };

            // Act.
            Action act = () => subject
                .Commands
                .Single(c => c.Name.Equals("decode", StringComparison.OrdinalIgnoreCase))
                .Execute(args);

            // Assert.
            act.ShouldNotThrow();
        }
    }
}