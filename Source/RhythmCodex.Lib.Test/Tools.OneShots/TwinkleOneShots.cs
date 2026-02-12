using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using RhythmCodex.Archs.Twinkle.Converters;
using RhythmCodex.Archs.Twinkle.Model;
using RhythmCodex.Archs.Twinkle.Streamers;
using RhythmCodex.Charts.Bms.Converters;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Tools.OneShots;

[TestFixture]
public class TwinkleOneShots : BaseIntegrationFixture
{
    // public override bool OutputFileFilter(string filename)
    // {
    //     return !filename.EndsWith(".wav", StringComparison.OrdinalIgnoreCase);
    // }

    [Test]
    [Explicit("This is a tool, not a test.")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iidx1st.zip", "bm2dx-1st")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iidxsubstream.zip", "bm2dx-sub")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iidx2nd.zip", "bm2dx-2nd")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iidx3rd.zip", "bm2dx-3rd")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iidx4th.zip", "bm2dx-4th")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iidx5th.zip", "bm2dx-5th")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iidx6th.zip", "bm2dx-6th")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iidx7th.zip", "bm2dx-7th")]
    [TestCase(@"/Volumes/RidgeportHDD/User Data/Bemani/Beatmania Non-PC/iidx8th.zip", "bm2dx-8th")]
    public void ExtractBms(string source, string target)
    {
        var streamer = Resolve<ITwinkleBeatmaniaStreamReader>();
        var decoder = Resolve<ITwinkleBeatmaniaDecoder>();

        using var stream = File.OpenRead(source);
        using var zipStream = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = zipStream.Entries.Single();
        using var entryStream = entry.Open();

        var options = new TwinkleDecodeOptions();

        var index = 0;

        var tasks = new List<Task>();

        foreach (var chunk in streamer.Read(entryStream, entry.Length, true))
        {
            if (IsCanceled)
                break;

            var idx = index;

            Log.WriteLine($"Working on chunk {idx}");

            tasks.Add(Task.Run(() =>
            {
                if (IsCanceled)
                    return;

                var archive = decoder.Decode(chunk, options);
                if (archive == null)
                    return;

                var title = $"{Alphabet.EncodeNumeric(idx, 4)}";
                var basePath = Path.Combine(target, title);

                this.WriteSet(archive.Charts, archive.Samples, idx, basePath, title, BmsChartType.Beatmania);
            }));

            index++;
        }

        Log.WriteLine($"Waiting for processing to finish");

        Task.WaitAll(tasks);
    }
}