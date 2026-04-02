using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.FileSystems.Cue.Processors;
using RhythmCodex.FileSystems.Cue.Streamers;
using RhythmCodex.FileSystems.Iso.Converters;
using RhythmCodex.FileSystems.Iso.Streamers;
using RhythmCodex.Games.Beatmania.Ps2.Converters;
using RhythmCodex.Sounds.Converters;
using Shouldly;

namespace RhythmCodex.Tools.OneShots;

[NonParallelizable]
public class BmPs2OneShots : BaseIntegrationFixture
{
    public const string ImageBasePath = "/Volumes/RidgeportHDD/User Data/Bemani/Playstation 2";

    /// <summary>
    /// Renders a BMS set from each song on a PS1 beatmania disc.
    /// </summary>
    [Test]
    [TestCase("beatmania US.iso", "ps2-bm-us")]
    [Explicit]
    public void ExtractBms(string source, string target)
    {
        const bool extractKeysounds = true;
        const bool extractCharts = true;
        const bool extractRawBlock = false;
        const bool extractBgm = true;
        const bool writeLogs = true;
        const float keyVolume = 0.75f;
        const float xaVolume = 0.75f;

        var audioDsp = Resolve<IAudioDsp>();

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

        var outfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), target);

        //
        // Find the executable file, which determines which game we are working with.
        //

        var exeName = cdFiles.First(x => x.Name != null && x.Name.StartsWith("./SL")).Name![2..^2];
        var formatDb = Resolve<IBeatmaniaPs2FormatDatabase>();
        var formatType = formatDb.GetTypeByExeName(exeName)
            ?? throw new Exception($"Could not determine type for {exeName}.");

        //
        // Create the adapter for the decoder to access the DVD file system.
        //

        Stream OpenFile(string fileName)
        {
            var actualFileName = $"./{fileName};1";
            var cdFile = cdFiles.Single(x => x.Name == actualFileName);
            return cdFile.Open();
        }

        //
        // Run the decoder.
        //

        var decoder = Resolve<IBeatmaniaPs2Decoder>();
        var chartSetConsolidator = Resolve<IBeatmaniaPs2ChartSetConsolidator>();

        foreach (var decodedSet in decoder.Decode(OpenFile, formatType))
        {
            var set = decodedSet;

            // RunAsync(() =>
            // {
            //     var setName = (set.Name ?? "")
            //         .Replace('\\', '_')
            //         .Replace('/', '_')
            //         .Trim();
            //     
            //     var outPath = Path.Combine(target, $"{set.SongId:d4}{(setName.Length == 0 ? "" : $" {setName}")}");
            //     var consolidatedSet = chartSetConsolidator.Consolidate(set);
            //
            //     this.WriteSet(new TestHelper.WriteSetConfig
            //     {
            //         OutPath = outPath,
            //         Charts = consolidatedSet.Charts,
            //         Sounds = consolidatedSet.Sounds,
            //     });
            // });
        }
    }
}