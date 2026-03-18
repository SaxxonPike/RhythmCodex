using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Archs.Psx.Converters;
using RhythmCodex.FileSystems.Cd.Streamers;
using RhythmCodex.FileSystems.Cue.Processors;
using RhythmCodex.FileSystems.Cue.Streamers;
using RhythmCodex.FileSystems.Iso.Converters;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Riff.Converters;
using RhythmCodex.Sounds.Riff.Streamers;
using RhythmCodex.Sounds.Xa.Converters;
using RhythmCodex.Sounds.Xa.Heuristics;
using RhythmCodex.Sounds.Xa.Models;
using Shouldly;

namespace RhythmCodex.Tools.OneShots;

public class BmPs1OneShots : BaseIntegrationFixture
{
    private static object[][] Paths => new object[][]
    {
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bm-eu.cue", "ps1-bm-eu"],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bm-jp-disc1.cue", "ps1-bm-jp-disc1"],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bm-jp-disc2.cue", "ps1-bm-jp-disc2"],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bm3rd.cue", "ps1-bm3rd"],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bm3rd-mini.cue", "ps1-bm3rd-mini"],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bm4th.cue", "ps1-bm4th"],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bm5th.cue", "ps1-bm5th"],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bm6th.cue", "ps1-bm6th"],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bmbest.cue", "ps1-bmbest"],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bmclub.cue", "ps1-bmclub"],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bmdct.cue", "ps1-bmdct"],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bmgotta.cue", "ps1-bmgotta"],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bmgotta2.cue", "ps1-bmgotta2"],
        ["/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bmsot.cue", "ps1-bmsot"]
    };

    /// <summary>
    /// Renders a BMS set from each song on a PS1 beatmania disc.
    /// </summary>
    [Test]
    [TestCaseSource(nameof(Paths))]
    [Explicit]
    public void ExtractBms(string source, string target)
    {
        // const bool extractAudio = true;
        // const bool extractCharts = true;
        // const bool extractRawBlock = false;

        //
        // Load in the CUE/BIN files.
        //

        using var cueStream = File.OpenRead(source);
        var cueReader = Resolve<ICueStreamReader>();
        var cue = cueReader.ReadCue(cueStream);

        var cueFolder = Path.GetDirectoryName(source);
        var track = cue.Tracks.Single();
        track.FileType.ShouldBe("BINARY");

        using var sectors = new CueCdSectorCollection(cue, f => File.OpenRead(Path.Combine(cueFolder!, f)));
        var cdFileDecoder = Resolve<IIsoCdFileDecoder>();

        var cdFiles = cdFileDecoder.Decode(sectors);

        Log.WriteLine("Files found:");
        foreach (var file in cdFiles)
            Log.WriteLine($"    {file.Name}");

        var outfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), target);

        //
        // Determine where BMDATA.PAK is located on the disc and load it.
        //

        Log.WriteLine("Loading BMDATA.PAK");
        using var bmDataPak = cdFiles.Single(f => f.Name == "./BMDATA.PAK;1").Open();

        //
        // Decode BMDATA.PAK.
        //

        var psxBeatmaniaDecoder = Resolve<IPsxBeatmaniaDecoder>();
        var bmDataPakFiles = psxBeatmaniaDecoder.DecodeBmData(bmDataPak, bmDataPak.Length);

        // for (var i = 0; i < bmDataPakFiles.Count; i++)
        //     this.WriteFile(bmDataPakFiles[i], Path.Combine(outfolder, $"bmdata{i:000}.bin"));

        //
        // Determine where MCHDATA.PAK is located on the disc and load it.
        // Because this is XA data, it should be loaded raw here to be processed.
        //

        Log.WriteLine("Loading MCHDATA.PAK");
        var mchDataCdFile = cdFiles.Single(f => f.Name == "./MCHDATA.PAK;1");
        using var mchDataPak = mchDataCdFile.OpenRaw();
        var modeTemp = new byte[0x10];
        mchDataPak.ReadExactly(modeTemp);
        var mchMode = modeTemp[0x0F];
        mchDataPak.Position = 0;

        //
        // Decode XA BGM.
        //

        List<XaChunk> xaChunks = [];

        var cdSectorsFactory = Resolve<ICdSectorCollectionFactory>();
        var isoInfoDecoder = Resolve<IIsoSectorInfoDecoder>();
        var streamFinder = Resolve<IXaCdStreamFinder>();

        switch (mchMode)
        {
            // case 1:
            // {
            //     using var mchDataMode1 = mchDataCdFile.Open();
            //     var mchData = new byte[mchDataMode1.Length];
            //     mchDataMode1.ReadExactly(mchData);
            //
            //     xaChunks.AddRange(mchData.Deinterleave(0x800, 4)
            //         .Select(block => new XaChunk { Data = block, Rate = 37800, Channels = 2 }));
            //     break;
            // }
            case 2:
            {
                var mchDataReader = cdSectorsFactory.Create(mchDataPak, mchDataPak.Length);
                var mchDataSectors = mchDataReader.Select(isoInfoDecoder.Decode);

                xaChunks.AddRange(streamFinder.FindMode2(mchDataSectors));
                break;
            }
            default:
            {
                Log.WriteLine("MCHDATA.PAK must be MODE2 in order to be extracted.");
                Log.WriteLine("You might be using a rip that makes it MODE1, which corrupts the audio data.");
                break;
            }
        }

        var decoder = Resolve<IXaDecoder>();
        var encoder = Resolve<IRiffPcm16SoundEncoder>();
        var writer = Resolve<IRiffStreamWriter>();

        var index = 0;

        foreach (var xa in xaChunks)
        {
            var decoded = decoder.Decode(xa);

            foreach (var sound in decoded)
            {
                sound![NumericData.Rate] = xa.Rate;
                var encoded = encoder.Encode(sound);
                if (!Directory.Exists(outfolder))
                    Directory.CreateDirectory(outfolder);

                using var outStream = new MemoryStream();
                writer.Write(outStream, encoded);
                outStream.Flush();
                File.WriteAllBytes(Path.Combine(outfolder, $"mchdata{index:000}.wav"), outStream.ToArray());
                index++;
            }
        }

        // foreach (var chunk in streamer.Read(entryStream))
        // {
        //     Log.WriteLine($"Working on chunk {index}");
        //
        //     var idx = index;
        //
        //     RunAsync(() =>
        //     {
        //         var archive = decoder.Decode(chunk, options);
        //
        //         Log.WriteLine($"Writing set for chunk {idx}");
        //
        //         var title = $"{Alphabet.EncodeNumeric(idx, 4)}";
        //         var basePath = Path.Combine(target, title);
        //
        //         if (extractRawBlock)
        //             this.WriteFile(archive.Chunk.Data, Path.Combine(basePath, $"{title}.bin"));
        //
        //         this.WriteSet(new TestHelper.WriteSetConfig
        //         {
        //             Charts = archive.Charts,
        //             Sounds = archive.Samples,
        //             ChartSetId = idx,
        //             OutPath = basePath,
        //             Title = title,
        //             ChartType = chartType,
        //             WriteCharts = extractCharts,
        //             WriteSounds = extractAudio
        //         });
        //     });
        //
        //     index++;
        // }
        //
        // WaitForAsyncTasks();
    }
}