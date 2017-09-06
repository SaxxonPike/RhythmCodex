using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using NUnit.Framework;

namespace RhythmCodex.Cli.Modules
{
    [TestFixture]
    public class SsqCliModuleTests : AppIntegrationFixture
    {
        [Test]
        [Explicit("Writes output to the desktop.")]
        public void Test1()
        {
            // Arrange.
            const string archiveFileName = "RhythmCodex.Cli.Data.freeze.zip";
            const string inputFileName = "freeze.ssq";
            
            var inputFile = GetArchiveResource(archiveFileName).Single().Value;
            FileSystem.WriteAllBytes(inputFileName, inputFile);
            
            var subject = AppContainer.Resolve<SsqCliModule>();
            var args = new Dictionary<string, string[]>
            {
                {string.Empty, new[] {inputFileName}}
            };

            // Act.
            subject
                .Commands
                .Single(c => c.Name.Equals("decode", StringComparison.OrdinalIgnoreCase))
                .Execute(args);
            
            // Assert.
            var outputFileName = $"{Path.DirectorySeparatorChar}freeze.ssq.sm";
            File.WriteAllBytes(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "output.sm"), 
                FileSystem.ReadAllBytes(outputFileName));
        }
    }
}