using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Charting.Models;
using RhythmCodex.Meta.Models;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Ssq.Streamers;

namespace RhythmCodex.Ssq.Integration
{
    [TestFixture]
    public class SsqEncodeIntegrationTests : BaseIntegrationFixture<SsqEncoder>
    {
        [Test]
        [NonParallelizable]
        [TestCase("Ssq.offset.zip")]
        [TestCase("Ssq.freeze.zip")]
        [TestCase("Ssq.shock.zip")]
        [TestCase("Ssq.solo.zip")]
        public void DecodeEncodeTest(string resource)
        {
            var reader = Resolve<ISsqStreamReader>();
            var decoder = Resolve<ISsqDecoder>();
            var encoder = Subject;

            var inBytes = GetArchiveResource(resource).First().Value;
            using var stream = new MemoryStream(inBytes);

            var inChunks = reader.Read(stream);
            var inChart = decoder.Decode(inChunks);
            var outChunks = encoder.Encode(inChart);

            var outMerged = Resolve<ISsqMerger>().Merge(inChunks, outChunks);
            var comparisons = inChunks.Zip(outMerged, (i, o) => new {Expected = i, Observed = o});

            foreach (var comparison in comparisons)
            {
                Console.WriteLine($"SSQ chunk " +
                                  $"[{comparison.Observed.Parameter0},{comparison.Observed.Parameter1}] " +
                                  $"[{comparison.Expected.Parameter0},{comparison.Expected.Parameter1}]");

                Console.WriteLine("Observed --");
                ConsoleWriteHexBlock(comparison.Observed.Data);
                
                Console.WriteLine("Expected --");
                ConsoleWriteHexBlock(comparison.Expected.Data);

                comparison.Observed.Should().BeEquivalentTo(comparison.Expected);
            }
        }
    
        [Test]
        public void EncodeSimpleSsq()
        {
            var chart = new Chart
            {
                [StringData.Difficulty] = "Medium",
                Events = new List<Event>
                {
                    new()
                    {
                        [NumericData.MetricOffset] = 0,
                        [NumericData.Bpm] = 120
                    },
                    new()
                    {
                        [NumericData.MetricOffset] = 1,
                        [FlagData.Note] = true,
                        [NumericData.Column] = 0,
                        [NumericData.Player] = 0
                    },
                    new()
                    {
                        [NumericData.MetricOffset] = 1.25,
                        [FlagData.Note] = true,
                        [NumericData.Column] = 1,
                        [NumericData.Player] = 0
                    },
                    new()
                    {
                        [NumericData.MetricOffset] = 1.5,
                        [FlagData.Note] = true,
                        [NumericData.Column] = 2,
                        [NumericData.Player] = 0
                    },
                    new()
                    {
                        [NumericData.MetricOffset] = 1.75,
                        [FlagData.Note] = true,
                        [NumericData.Column] = 3,
                        [NumericData.Player] = 0
                    },
                    new()
                    {
                        [NumericData.MetricOffset] = 2,
                        [FlagData.Note] = true,
                        [NumericData.Column] = 0,
                        [NumericData.Player] = 0
                    },
                }
            };

            var observed = Subject.Encode(new[] {chart});

            var reversed = Resolve<ISsqDecoder>().Decode(observed);

            // using var stream = this.OpenWrite("ssq\\ssq.bin");
            // Resolve<SsqStreamWriter>().Write(stream, observed);
            // stream.Flush();
        }
    }
}