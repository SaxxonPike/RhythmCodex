using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Bms.Converters;
using RhythmCodex.Bms.Streamers;
using RhythmCodex.Dsp;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Processing;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Twinkle.Converters;
using RhythmCodex.Twinkle.Model;
using RhythmCodex.Twinkle.Streamers;
using RhythmCodex.Wav.Converters;

namespace RhythmCodex.Twinkle.Integration
{
    public class TwinkleBeatmaniaIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [Explicit("wip")]
        public void Test1()
        {
            var streamer = Resolve<ITwinkleBeatmaniaStreamReader>();

            using (var stream = File.OpenRead(@"Z:\Bemani\Beatmania Non-PC\iidx8th.zip"))
            using (var zipStream = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                var entry = zipStream.Entries.Single();
                using (var entryStream = entry.Open())
                {
                    var chunks = streamer.Read(entryStream, stream.Length);
                    var chunk = chunks.Skip(1).First();
                    File.WriteAllBytes(@"c:\users\saxxon\desktop\twinkle.bin", chunk.Data);
                }
            }
        }

        [Test]
        [Explicit("wip")]
        public void Test2()
        {
            // Arrange.
            var data = GetArchiveResource("Twinkle.8th.zip")
                .First()
                .Value;
            var streamer = Resolve<ITwinkleBeatmaniaStreamReader>();
            var decoder = Resolve<ITwinkleBeatmaniaDecoder>();
            var bmsEncoder = Resolve<IBmsEncoder>();
            var bmsWriter = Resolve<IBmsStreamWriter>();

            // Act.
            var chunk = streamer.Read(new MemoryStream(data), data.Length, false).First();
            var archive = decoder.Decode(chunk);

            // Assert.
            foreach (var sound in archive.Samples.Where(s => s.Samples.Any()))
            {
                this.WriteSound(sound, Path.Combine("bmiidx", $"{Alphabet.EncodeAlphanumeric((int)sound[NumericData.Id], 4)}.wav"));
            }

            foreach (var chart in archive.Charts)
            {
                chart.PopulateMetricOffsets();
                using (var outStream =
                    this.OpenWrite(Path.Combine("bmiidx", $"{(int) chart[NumericData.ByteOffset]}.bms")))
                {
                    bmsWriter.Write(outStream, bmsEncoder.Encode(chart));
                    outStream.Flush();
                }
            }
        }
        
        [Test]
        [Explicit("wip")]
        public void Test3()
        {
            // Arrange.
            var data = GetArchiveResource("Twinkle.8th.zip")
                .First()
                .Value;
            var streamer = Resolve<ITwinkleBeatmaniaStreamReader>();
            var decoder = Resolve<ITwinkleBeatmaniaDecoder>();
            var renderer = Resolve<IChartRenderer>();
            var dsp = Resolve<IAudioDsp>();

            // Act.
            var chunk = streamer.Read(new MemoryStream(data), data.Length, false).First();
            var archive = decoder.Decode(chunk);
            var rendered = dsp.Normalize(renderer.Render(archive.Charts[1].Events, archive.Samples, 44100), 1.0f);

            // Assert.
            this.WriteSound(rendered, Path.Combine($"twinkle.wav"));
        }
        
        [Test]
        [Explicit("wip")]
        public void Test4()
        {
            var streamer = Resolve<ITwinkleBeatmaniaStreamReader>();
            var decoder = Resolve<ITwinkleBeatmaniaDecoder>();
            var renderer = Resolve<IChartRenderer>();
            var dsp = Resolve<IAudioDsp>();

            using (var stream = File.OpenRead(@"D:\iidx8th.zip"))
            using (var zipStream = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                var entry = zipStream.Entries.Single();
                using (var entryStream = entry.Open())
                {
                    var chunks = streamer.Read(entryStream, stream.Length, true);

                    foreach (var chunk in chunks.AsParallel().Skip(153).Take(1))
                    {
                        if (chunk.Data[0x2000] != 0 || chunk.Data[0x2001] != 0 || chunk.Data[0x2002] == 0 ||
                            chunk.Data[0x2003] != 0)
                            continue;

                        var archive = decoder.Decode(chunk);
                        foreach (var chart in archive.Charts.Take(1))
                        {
                            var rendered = dsp.Normalize(renderer.Render(archive.Charts.First().Events, archive.Samples, 44100), 1.0f);
                            this.WriteSound(rendered, Path.Combine($"twinkle\\{chunk.Index:D4}_{(int) chart[NumericData.Id]:D2}.wav"));
                        }
                    }
                }
            }
        }
    }
}