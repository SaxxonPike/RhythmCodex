using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using RhythmCodex.Archs.Firebeat.Converters;
using RhythmCodex.Archs.Firebeat.Models;
using RhythmCodex.Archs.Firebeat.Streamers;
using RhythmCodex.Charts.Bms.Converters;
using RhythmCodex.Infrastructure;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Wav.Converters;
using RhythmCodex.Sounds.Wav.Models;

namespace RhythmCodex.Tools.OneShots;

public class FirebeatOneShots : BaseIntegrationFixture
{
    private static object[][] Paths => new object[][]
    {
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iii.zip", "bm3-1st", BmsChartType.Beatmania],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iii6thappend.zip", "bm3-6th", BmsChartType.Beatmania],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iii7thappend.zip", "bm3-7th", BmsChartType.Beatmania],
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
        var streamer = Resolve<IFirebeatChunkStreamReader>();
        var decoder = Resolve<IFirebeatDecoder>();

        using var stream = File.OpenRead(source);
        using var zipStream = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = zipStream.Entries.Single();
        using var entryStream = entry.Open();

        var options = new FirebeatDecodeOptions();

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
    /// Renders a GST using each chart from each chunk within a Firebeat system HDD.
    /// </summary>
    [Test]
    [TestCaseSource(nameof(Paths))]
    [Explicit]
    public void RenderGst(string source, string target, BmsChartType chartType)
    {
        var streamer = Resolve<IFirebeatChunkStreamReader>();
        var decoder = Resolve<IFirebeatDecoder>();
        var renderer = Resolve<IChartRenderer>();
        var dsp = Resolve<IAudioDsp>();
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