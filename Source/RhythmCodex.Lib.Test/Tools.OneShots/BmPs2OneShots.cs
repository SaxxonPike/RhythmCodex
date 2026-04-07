using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.FileSystems.Iso.Converters;
using RhythmCodex.FileSystems.Iso.Streamers;
using RhythmCodex.Games.Beatmania.Ps2.Converters;
using RhythmCodex.Games.Beatmania.Ps2.Services;

namespace RhythmCodex.Tools.OneShots;

public class BmPs2OneShots : BaseIntegrationFixture
{
    public const string ImageBasePath = "/Volumes/RidgeportHDD/User Data/Bemani/Playstation 2";

    /// <summary>
    /// Renders a BMS set from each song on a PS1 beatmania disc.
    /// </summary>
    [Test]
    [TestCase("beatmania IIDX 3rd style.iso", "ps2-bm-3rd")]
    [TestCase("beatmania IIDX 4th style.iso", "ps2-bm-4th")]
    [TestCase("beatmania IIDX 5th style.iso", "ps2-bm-5th")]
    [TestCase("beatmania IIDX 6th style.iso", "ps2-bm-6th")]
    [TestCase("beatmania IIDX 7th style.iso", "ps2-bm-7th")]
    [TestCase("beatmania IIDX 8th style.iso", "ps2-bm-8th")]
    [TestCase("beatmania IIDX 9th style.iso", "ps2-bm-9th")]
    [TestCase("beatmania IIDX 10th style.iso", "ps2-bm-10th")]
    [TestCase("beatmania IIDX 11th style.iso", "ps2-bm-11th")]
    [TestCase("beatmania IIDX 12th style.iso", "ps2-bm-12th")]
    [TestCase("beatmania IIDX 13th style.iso", "ps2-bm-13th")]
    [TestCase("beatmania IIDX 14th style.iso", "ps2-bm-14th")]
    [TestCase("beatmania IIDX 15th style.iso", "ps2-bm-15th")]
    [TestCase("beatmania US.iso", "ps2-bm-us")]
    [Explicit]
    public void ExtractBms(string source, string target)
    {
        const bool extractKeysounds = true;
        const bool extractCharts = true;
        const bool extractRawBlock = false;
        const bool writeLogs = true;
        const float keyVolume = 0.7f;

        //
        // Load in the ISO files.
        //

        var sourcePath = Path.Combine(ImageBasePath, source);
        using var isoStream = File.OpenRead(sourcePath);
        var isoSectorCollectionFactory = Resolve<IIsoSectorCollectionFactory>();
        var sectors = isoSectorCollectionFactory.Create(isoStream, isoStream.Length);

        var cdFileDecoder = Resolve<IIsoCdFileDecoder>();
        var cdFiles = cdFileDecoder.Decode(sectors);

        Log.WriteLine("Files found:");
        foreach (var file in cdFiles)
            Log.WriteLine($"    {file.Name}");

        //
        // Find the executable file, which determines which game we are working with.
        //

        var exeName = cdFiles.First(x => x.Name != null && x.Name.StartsWith("./SL")).Name![2..^2];
        var formatDb = Resolve<IBeatmaniaPs2FormatDatabase>();
        var formatType = formatDb.GetTypeByExeName(exeName)
            ?? throw new Exception($"Could not determine type for {exeName}.");

        //
        // Run the decoder.
        //

        var decoder = Resolve<IBeatmaniaPs2Service>();
        var chartSetConsolidator = Resolve<IBeatmaniaPs2ChartSetConsolidator>();

        foreach (var decodedSet in decoder.Decode(OpenFile, formatType))
        {
            var set = decodedSet;

            RunAsync(() =>
            {
                var setName = (set.Name ?? "")
                    .Replace('\\', '_')
                    .Replace('/', '_')
                    .Replace('*', '_')
                    .Replace('?', '_')
                    .Replace('&', '_')
                    .Replace('<', '_')
                    .Replace('>', '_')
                    .Replace('|', '_')
                    .Trim();
                
                var outPath = Path.Combine(target, $"{set.SongId:d4}{(setName.Length == 0 ? "" : $" {setName}")}");
                var consolidatedSet = chartSetConsolidator.Consolidate(set);
            
                Log.WriteLine($"Writing set for song {set.SongId}");
                this.WriteSet(new TestHelper.WriteSetConfig
                {
                    OutPath = outPath,
                    Charts = consolidatedSet.Charts,
                    Sounds = consolidatedSet.Sounds,
                    WriteCharts = extractCharts,
                    WriteSounds = extractKeysounds,
                    KeysoundVolume = keyVolume
                });
            });
        }

        return;
        
        //
        // Adapter for the decoder to access the DVD file system.
        //

        Stream OpenFile(string fileName)
        {
            var actualFileName = $"./{fileName};1";
            var cdFile = cdFiles.Single(x => x.Name == actualFileName);
            return cdFile.Open();
        }
    }
}