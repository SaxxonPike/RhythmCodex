using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Archs.Djmain.Converters;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Archs.Djmain.Streamers;
using RhythmCodex.Charts.Bms.Converters;
using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Wav.Models;

namespace RhythmCodex.Tools.OneShots;

public class DjmainOneShots : BaseIntegrationFixture
{
    private static object[][] Paths => new object[][]
    {
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm1stmix.zip", "bm1-1st", BmsChartType.Beatmania],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm2ndmix.zip", "bm1-2nd", BmsChartType.Beatmania],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm3rdmix.zip", "bm1-3rd", BmsChartType.Beatmania],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm4thmix.zip", "bm1-4th", BmsChartType.Beatmania],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm5thmix.zip", "bm1-5th", BmsChartType.Beatmania],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm6thmix.zip", "bm1-6th", BmsChartType.Beatmania],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bm7thmix.zip", "bm1-7th", BmsChartType.Beatmania],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bmclubmx.zip", "bm1-club", BmsChartType.Beatmania],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bmcompmx.zip", "bm1-comp", BmsChartType.Beatmania],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bmcompm2.zip", "bm1-comp2", BmsChartType.Beatmania],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bmcorerm.zip", "bm1-core", BmsChartType.Beatmania],
        ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/bmfinal.zip", "bm1-final", BmsChartType.Beatmania],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Popn Non-PC/popn1.zip", "popn1", BmsChartType.Popn],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Popn Non-PC/popn2.zip", "popn2", BmsChartType.Popn],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Popn Non-PC/popn3.zip", "popn3", BmsChartType.Popn]
    };
    
    /// <summary>
    /// Renders a BMS set from each chunk within a Djmain system HDD.
    /// </summary>
    [Test]
    [TestCaseSource(nameof(Paths))]
    [Explicit]
    public void ExtractBms(string source, string target, BmsChartType chartType)
    {
        var streamer = Resolve<IDjmainChunkStreamReader>();
        var decoder = Resolve<IDjmainDecoder>();

        using var stream = File.OpenRead(source);
        using var zipStream = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = zipStream.Entries.Single();
        using var entryStream = entry.Open();

        var options = new DjmainDecodeOptions
        {
            SwapStereo = chartType == BmsChartType.Beatmania
        };

        var index = 0;

        foreach (var chunk in streamer.Read(entryStream))
        {
            SetProgress($"Working on chunk {index}");

            var idx = index;

            RunAsync(() =>
            {
                var archive = decoder.Decode(chunk, options);
                if (archive != null)
                {
                    SetProgress($"Writing set for chunk {idx}");
                    var title = $"{Alphabet.EncodeNumeric(idx, 4)}";
                    var basePath = Path.Combine(target, title);

                    this.WriteSet(new TestHelper.WriteSetConfig
                    {
                        Charts = archive.Charts,
                        Sounds = archive.Samples,
                        ChartSetId = idx,
                        OutPath = basePath,
                        Title = title,
                        ChartType = chartType
                    });
                }
            });

            index++;

            Yield();
        }

        WaitForAsyncTasks();
    }

    /// <summary>
    /// Renders a GST using each chart from each chunk within a Djmain system HDD.
    /// </summary>
    [Test]
    [TestCaseSource(nameof(Paths))]
    [Explicit]
    public void RenderGst(string source, string target, BmsChartType chartType)
    {
        var streamer = Resolve<IDjmainChunkStreamReader>();
        var decoder = Resolve<IDjmainDecoder>();
        var hashes = new HashSet<int>();

        using var stream = File.OpenRead(source);
        using var zipStream = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = zipStream.Entries.Single();
        using var entryStream = entry.Open();

        var options = new DjmainDecodeOptions
        {
            DoNotConsolidateSamples = true,
            SwapStereo = chartType == BmsChartType.Beatmania
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

                if (archive == null)
                    return;

                Log.WriteLine($"Rendering for chunk {idx}");

                this.RenderSet(new TestHelper.RenderSetConfig
                {
                    Charts = archive.Charts,
                    Sounds = archive.Samples,
                    RenderOptions = renderOptions,
                    ChartSetId = idx,
                    OutPath = target,
                    Title = $"{idx:D4}",
                    Hashes = hashes
                });
            });

            index++;
        }

        WaitForAsyncTasks();
    }
}