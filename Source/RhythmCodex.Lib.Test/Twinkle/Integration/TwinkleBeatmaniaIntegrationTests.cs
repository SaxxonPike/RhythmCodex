using System.IO;
using System.IO.Compression;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Beatmania.Streamers;
using RhythmCodex.Bms.Converters;
using RhythmCodex.Bms.Streamers;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Twinkle.Converters;
using RhythmCodex.Twinkle.Streamers;
using RhythmCodex.Wav.Converters;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Twinkle.Integration;

public class TwinkleBeatmaniaIntegrationTests : BaseIntegrationFixture
{
    [Test]
    [Explicit("wip")]
    public void Test1()
    {
        var streamer = Resolve<ITwinkleBeatmaniaStreamReader>();

        using var stream = File.OpenRead(@"Z:\Bemani\Beatmania Non-PC\iidx8th.zip");
        using var zipStream = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = zipStream.Entries.Single();
        using var entryStream = entry.Open();
        var chunks = streamer.Read(entryStream, entry.Length);
        var chunk = chunks.Skip(1).First();
        File.WriteAllBytes(@"c:\users\saxxon\desktop\twinkle.bin", chunk.Data);
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
            using var outStream =
                this.OpenWrite(Path.Combine("bmiidx", $"{(int) chart[NumericData.ByteOffset]}.bms"));
            bmsWriter.Write(outStream, bmsEncoder.Encode(chart));
            outStream.Flush();
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
        var options = new ChartRendererOptions();

        // Act.
        var chunk = streamer.Read(new MemoryStream(data), data.Length, false).First();
        var archive = decoder.Decode(chunk);
        var rendered = dsp.Normalize(renderer.Render(archive.Charts[1].Events, archive.Samples, options), 1.0f, true);

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
        var options = new ChartRendererOptions();

        using var stream = File.OpenRead(@"Z:\Bemani\Beatmania Non-PC\iidx7th.zip");
        using var zipStream = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = zipStream.Entries.Single();
        using var entryStream = entry.Open();
        var chunks = streamer.Read(entryStream, entry.Length, true);

        foreach (var chunk in chunks.AsParallel())
        {
            var archive = decoder.Decode(chunk);
            if (archive == null)
                continue;

            foreach (var chart in archive.Charts.AsParallel())
            {
                var rendered = dsp.Normalize(renderer.Render(chart.Events, archive.Samples, options), 1.0f, false);
                this.WriteSound(rendered, Path.Combine($"twinkle7\\{chunk.Index:D4}_{(int) chart[NumericData.Id]:D2}.wav"));
            }
        }
    }
        
    [Test]
    [Explicit("wip")]
    public void Test5()
    {
        var streamer = Resolve<ITwinkleBeatmaniaStreamReader>();
        using var stream = File.OpenRead(@"Z:\Bemani\Beatmania Non-PC\iidx1st.zip");
        using var zipStream = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = zipStream.Entries.Single();
        using var entryStream = entry.Open();
        var chunks = streamer.Read(entryStream, entry.Length, true);

        foreach (var chunk in chunks.AsParallel())
            this.WriteFile(chunk.Data, Path.Combine("twinkle1st", $"{chunk.Index:D4}.twinkle"));
    }

    [Test]
    [Explicit("wip")]
    public void Test6()
    {
        var streamer = Resolve<ITwinkleBeatmaniaStreamReader>();
        var decoder = Resolve<ITwinkleBeatmaniaDecoder>();
        var chartWriter = Resolve<IBeatmaniaPc1StreamWriter>();
        using var stream = File.OpenRead(@"Z:\Bemani\Beatmania Non-PC\iidx1st.zip");
        using var zipStream = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = zipStream.Entries.Single();
        using var entryStream = entry.Open();
        var chunks = streamer.Read(entryStream, entry.Length, true);

        foreach (var chunk in chunks.Take(1))
        {
            var bpc = decoder.MigrateToBemaniPc(chunk);
            using var mem = new MemoryStream();
            chartWriter.Write(mem, bpc.Charts);
            mem.Flush();
            this.WriteFile(mem.ToArray(), $"{chunk.Index:D4}.1");
        }
    }
}