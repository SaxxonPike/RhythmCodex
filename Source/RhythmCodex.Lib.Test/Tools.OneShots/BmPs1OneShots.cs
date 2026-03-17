using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Archs.Psx.Converters;
using RhythmCodex.Archs.Psx.Streamers;
using RhythmCodex.Extensions;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.FileSystems.Cd.Streamers;
using RhythmCodex.FileSystems.Cue.Processors;
using RhythmCodex.FileSystems.Cue.Streamers;
using RhythmCodex.FileSystems.Iso.Converters;
using RhythmCodex.FileSystems.Iso.Streamers;
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
        ["/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bmgotta2.cue", "ps1-bmgotta2"],
        // ["/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bmsot.cue", "ps1-bmsot"]
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

        using var sectors = new CueCdSectors(cue, f => File.OpenRead(Path.Combine(cueFolder!, f)));
        var cdFileDecoder = Resolve<IIsoCdFileDecoder>();

        var cdFiles = cdFileDecoder.Decode(sectors);

        Log.WriteLine("Files found:");
        foreach (var file in cdFiles)
            Log.WriteLine($"    {file.Name}");

        var outfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "xa");

        //
        // Determine where BMDATA.PAK is located on the disc and load it.
        //

        Log.WriteLine("Loading BMDATA.PAK");
        using var bmDataPak = cdFiles.Single(f => f.Name == "./BMDATA.PAK;1").Open();

        //
        // Decode BMDATA.PAK.
        //

        var bmDataPakStreamReader = Resolve<IBmDataPakStreamReader>();
        Span<byte> bmDataTemp = stackalloc byte[16];
        var bmDataPakFiles = new List<ReadOnlyMemory<byte>>();

        for (var i = 0; i < bmDataPak.Length - 0x7FF; i += 0x800)
        {
            bmDataPak.Position = i;
            
            if (bmDataPak.Position == 0x1388000)
            {
                
            }

            
            bmDataPak.ReadExactly(bmDataTemp);
            if (ReadInt32LittleEndian(bmDataTemp) == ReadInt32LittleEndian(bmDataTemp[8..]) &&
                ReadInt32LittleEndian(bmDataTemp) is > 0 and < 32768 &&
                ReadInt32LittleEndian(bmDataTemp[4..]) is > 0 and < 1024 &&
                ReadInt32LittleEndian(bmDataTemp[8..]) is > 0 and < 32768 &&
                ReadInt32LittleEndian(bmDataTemp[12..]) > 0)
            {
                bmDataPak.Position = i;
                var bmDataPakDirectory = bmDataPakStreamReader.ReadDirectory(bmDataPak);
                bmDataPakFiles.AddRange(bmDataPakStreamReader.ReadEntries(bmDataPak, bmDataPakDirectory));
            }
        }
        
        for (var i = 0; i < bmDataPakFiles.Count; i++)
            this.WriteFile(bmDataPakFiles[i], Path.Combine(outfolder, $"bmdata{i:000}.bin"));

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

        //
        // Decode XA BGM. The data could be either MODE 1 or MODE 2. These require
        // different methods of splitting.
        //

        List<XaChunk> xaChunks = [];

        switch (mchMode)
        {
            case 1:
            {
                using var mchDataPak2 = mchDataCdFile.Open();
                var mchDataBytes = new byte[mchDataPak2.Length];
                mchDataPak2.ReadExactly(mchDataBytes);
                xaChunks.AddRange(mchDataBytes.Deinterleave(0x800, 4)
                    .Select(block => new XaChunk { Data = block, Rate = 37800, Channels = 2 }));
                break;
            }
            case 2:
            {
                mchDataPak.Position = 0;
                var isoInfoDecoder = Resolve<IIsoSectorInfoDecoder>();
                var isoSectorStreamReader = Resolve<IIsoSectorStreamReader>();

                var mchDataReader = isoSectorStreamReader.Read(
                    mchDataPak,
                    (int)mchDataPak.Length,
                    false,
                    true,
                    mchMode
                );

                var streamFinder = Resolve<IXaIsoStreamFinder>();
                var mchDataSectors = mchDataReader
                    .Select(x =>
                    {
                        // var newData = x.Data.ToArray();
                        // newData[0x0F] = 0x02;
                        // newData[0x12] |= 0x20;
                        // var info = isoInfoDecoder.Decode(x.Number, x.Data);
                        // return info;
                        return isoInfoDecoder.Decode(x);
                    });

                xaChunks.AddRange(streamFinder.FindMode2(mchDataSectors));
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