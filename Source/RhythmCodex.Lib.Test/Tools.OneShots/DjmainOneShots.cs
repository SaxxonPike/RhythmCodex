using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using RhythmCodex.Archs.Djmain.Converters;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Archs.Djmain.Streamers;
using RhythmCodex.Charts.Bms.Converters;
using RhythmCodex.Infrastructure;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Wav.Converters;
using RhythmCodex.Sounds.Wav.Models;

namespace RhythmCodex.Tools.OneShots;

public class DjmainOneShots : BaseIntegrationFixture
{
    /// <summary>
    /// Renders a BMS set from each chunk within a Djmain system HDD.
    /// </summary>
    [Test]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm1stmix.zip", "bm1-1st", BmsChartType.Beatmania)]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm2ndmix.zip", "bm1-2nd", BmsChartType.Beatmania)]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm3rdmix.zip", "bm1-3rd", BmsChartType.Beatmania)]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm4thmix.zip", "bm1-4th", BmsChartType.Beatmania)]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm5thmix.zip", "bm1-5th", BmsChartType.Beatmania)]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm6thmix.zip", "bm1-6th", BmsChartType.Beatmania)]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm7thmix.zip", "bm1-7th", BmsChartType.Beatmania)]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bmclubmx.zip", "bm1-club", BmsChartType.Beatmania)]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bmcompmx.zip", "bm1-comp", BmsChartType.Beatmania)]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bmcompm2.zip", "bm1-comp2", BmsChartType.Beatmania)]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bmcorerm.zip", "bm1-core", BmsChartType.Beatmania)]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bmfinal.zip", "bm1-final", BmsChartType.Beatmania)]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Popn Non-PC/popn1.zip", "popn1", BmsChartType.Popn)]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Popn Non-PC/popn2.zip", "popn2", BmsChartType.Popn)]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Popn Non-PC/popn3.zip", "popn3", BmsChartType.Popn)]
    [Explicit]
    public void ExtractBms(string source, string target, BmsChartType chartType)
    {
        var streamer = Resolve<IDjmainChunkStreamReader>();
        var decoder = Resolve<IDjmainDecoder>();

        using var stream = File.OpenRead(source);
        using var zipStream = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = zipStream.Entries.Single();
        using var entryStream = entry.Open();

        var options = new DjmainDecodeOptions();

        var index = 0;
        var tasks = new List<Task>();

        foreach (var chunk in streamer.Read(entryStream))
        {
            Log.WriteLine($"Working on chunk {index}");

            var idx = index;

            RunAsync(() =>
            {
                var archive = decoder.Decode(chunk, options);
                if (archive != null)
                {
                    Log.WriteLine($"Writing set for chunk {idx}");
                    var title = $"{Alphabet.EncodeNumeric(idx, 4)}";
                    var basePath = Path.Combine(target, title);
                    
                    this.WriteSet(archive.Charts, archive.Samples, basePath, title, chartType);
                }
            });

            index++;
        }

        Task.WaitAll(tasks.ToArray());
    }

    /// <summary>
    /// Renders a GST using each chart from each chunk within a Djmain system HDD.
    /// </summary>
    [Test]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm1stmix.zip", "bm1-1st")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm2ndmix.zip", "bm1-2nd")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm3rdmix.zip", "bm1-3rd")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm4thmix.zip", "bm1-4th")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm5thmix.zip", "bm1-5th")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm6thmix.zip", "bm1-6th")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm7thmix.zip", "bm1-7th")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bmclubmx.zip", "bm1-club")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bmcompmx.zip", "bm1-comp")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bmcompm2.zip", "bm1-comp2")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bmcorerm.zip", "bm1-core")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bmfinal.zip", "bm1-final")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Popn Non-PC/popn1.zip", "popn1")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Popn Non-PC/popn2.zip", "popn2")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Popn Non-PC/popn3.zip", "popn3")]
    [Explicit]
    public void RenderGst(string source, string target)
    {
        var streamer = Resolve<IDjmainChunkStreamReader>();
        var decoder = Resolve<IDjmainDecoder>();
        var renderer = Resolve<IChartRenderer>();
        var dsp = Resolve<IAudioDsp>();
        var hashes = new HashSet<int>();

        using var stream = File.OpenRead(source);
        using var zipStream = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = zipStream.Entries.Single();
        using var entryStream = entry.Open();

        var options = new DjmainDecodeOptions
        {
            DoNotConsolidateSamples = true
        };

        var renderOptions = new ChartRendererOptions
        {
            SwapStereo = true
        };

        var index = 0;

        foreach (var chunk in streamer.Read(entryStream))
        {
            Log.WriteLine($"Working on chunk {index}");

            var idx = index;

            RunAsync(() =>
            {
                if (IsCanceled)
                    return;

                var archive = decoder.Decode(chunk, options);

                foreach (var chart in archive.Charts)
                {
                    var id = (int)chart[NumericData.Id]!.Value;
                    var title = $"{Alphabet.EncodeNumeric(idx, 4)}-{Alphabet.EncodeNumeric(id, 2)}";
                    var path = Path.Combine(target, $"{title}.wav");

                    Log.WriteLine($"Rendering chart {id} for chunk {idx}");

                    var rendered = renderer.Render(chart, archive.Samples, renderOptions);
                    var renderedHash = rendered.CalculateSampleHash();

                    if (!hashes.Add(renderedHash))
                        continue;

                    var normalized = dsp.Normalize(rendered, 1.0f, true);
                    this.WriteSound(normalized, path);
                }
            });

            index++;
        }
    }
}