using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using RhythmCodex.Djmain.Converters;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Djmain.Streamers;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.OneShots;

public class DjmainOneShots : BaseIntegrationFixture
{
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
    [Explicit]
    public void ExtractBms(string source, string target)
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
            TestContext.Out.WriteLine($"Working on chunk {index}");

            var idx = index;

            tasks.Add(Task.Run(() =>
            {
                var archive = decoder.Decode(chunk, options);
                if (archive != null)
                {
                    TestContext.Out.WriteLine($"Writing set for chunk {idx}");
                    var title = $"{Alphabet.EncodeNumeric(idx, 4)}";
                    var basePath = Path.Combine(target, title);
                    this.WriteSet(archive.Charts, archive.Samples, basePath, title);
                }
            }));

            index++;
        }

        Task.WaitAll(tasks.ToArray());
    }
}