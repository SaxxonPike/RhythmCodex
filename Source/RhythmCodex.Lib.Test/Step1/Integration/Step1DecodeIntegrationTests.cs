using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Attributes;
using RhythmCodex.Step1.Converters;
using RhythmCodex.Step1.Streamers;
using RhythmCodex.Step2.Converters;
using RhythmCodex.Step2.Streamers;

namespace RhythmCodex.Step1.Integration
{
    [TestFixture]
    public class Step1DecodeIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [TestCase("picky")]
        [TestCase("solo")]
        [TestCase("2nd")]
        public void DecodeAllCharts(string chartName)
        {
            var streamReader = Resolve<IStep1StreamReader>();
            var decoder = Resolve<IStep1Decoder>();
            
            var data = GetArchiveResource($"Step1.{chartName}.zip")
                .First()
                .Value;

            var chunk = streamReader.Read(new MemoryStream(data));
            var charts = decoder.Decode(chunk);

            foreach (var chart in charts)
            {
                TestContext.Out.WriteLine($"{chart["Difficulty"]} {chart["Type"]} {chart["Description"]}");
            }
        }

        [Explicit]
        [Test]
        public void DecodeFolder()
        {
            var streamReader = Resolve<IStep1StreamReader>();
            var decoder = Resolve<IStep1Decoder>();
            
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ddr3mp");
            var files = Directory.GetFiles(folder, "*.step");

            foreach (var file in files)
            {
                TestContext.Out.WriteLine(file);
                var data = File.ReadAllBytes(file);
                var chunk = streamReader.Read(new MemoryStream(data));
                var charts = decoder.Decode(chunk);

                foreach (var chart in charts)
                {
                    TestContext.Out.WriteLine($"{chart["Difficulty"]} {chart["Type"]} {chart["Description"]}");
                }                
            }
        }
    }
}