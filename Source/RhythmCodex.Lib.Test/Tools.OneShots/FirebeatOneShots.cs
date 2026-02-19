using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using RhythmCodex.Archs.Firebeat.Converters;
using RhythmCodex.Archs.Firebeat.Models;
using RhythmCodex.Archs.Firebeat.Streamers;
using RhythmCodex.Charts.Bms.Converters;
using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Wav.Models;

namespace RhythmCodex.Tools.OneShots;

public class FirebeatOneShots : BaseIntegrationFixture
{
    private static object[][] Paths => new object[][]
    {
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iii.zip", "bm3-1st", BmsChartType.Beatmania],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iii6thappend.zip", "bm3-6th", BmsChartType.Beatmania],
        //["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iii7thappend.zip", "bm3-7th", BmsChartType.Beatmania],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iiicore.zip", "bm3-core", BmsChartType.Beatmania],
        ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iiifinal.zip", "bm3-final", BmsChartType.Beatmania]
    };

    /// <summary>
    /// Renders a BMS set from each chunk within a Firebeat system HDD.
    /// </summary>
    [Test]
    [TestCaseSource(nameof(Paths))]
    [Explicit]
    public void ExtractBms(string source, string target, BmsChartType chartType)
    {
        const bool extractAudio = true;
        const bool extractCharts = true;
        const bool extractRawBlock = false;
        const bool logChartInfo = true;

        var streamer = Resolve<IFirebeatChunkStreamReader>();
        var decoder = Resolve<IFirebeatDecoder>();

        using var stream = File.OpenRead(source);
        using var zipStream = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = zipStream.Entries.Single();
        using var entryStream = entry.Open();

        var options = new FirebeatDecodeOptions();

        var index = 0;

        foreach (var chunk in streamer.Read(entryStream))
        {
            Log.WriteLine($"Working on chunk {index}");

            var idx = index;

            RunAsync(() =>
            {
                var archive = decoder.Decode(chunk, options);

                var title = $"{Alphabet.EncodeNumeric(idx, 4)}";
                var basePath = Path.Combine(target, title);

                if (extractRawBlock)
                    this.WriteFile(archive.Chunk.Data, Path.Combine(basePath, $"{title}.bin"));

                if (logChartInfo)
                {
                    Log.WriteLine(JsonSerializer.Serialize(new
                    {
                        Idx = idx,
                        Bpm = archive.RawCharts
                            .SelectMany(c => new[] { c.Header.MinBpm, c.Header.MaxBpm })
                            .Distinct()
                            .ToHashSet(),
                        NoteCounts = archive.RawCharts
                            .ToDictionary(
                                c => c.Id,
                                c => new[] { c.Header.MaxNoteCount1p, c.Header.MaxNoteCount2p }
                            )
                    }));
                }

                this.WriteSet(new TestHelper.WriteSetConfig
                {
                    Charts = archive.Charts,
                    Sounds = archive.Samples,
                    ChartSetId = idx,
                    OutPath = basePath,
                    Title = title,
                    ChartType = chartType,
                    WriteCharts = extractCharts,
                    WriteSounds = extractAudio
                });
            });

            index++;
        }

        WaitForAsyncTasks();
    }

    /// <summary>
    /// Renders a GST using each chart from each chunk within a Firebeat system HDD.
    /// </summary>
    [Test]
    [TestCaseSource(nameof(Paths))]
    [Explicit]
    public void RenderGst(string source, string target, BmsChartType chartType)
    {
        var streamer = Resolve<IFirebeatChunkStreamReader>();
        var decoder = Resolve<IFirebeatDecoder>();
        var hashes = new HashSet<int>();

        using var stream = File.OpenRead(source);
        using var zipStream = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = zipStream.Entries.Single();
        using var entryStream = entry.Open();

        var options = new FirebeatDecodeOptions
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
                    Hashes = hashes,
                    FirstOnly = true
                });
            });

            index++;
        }

        WaitForAsyncTasks();
    }
}