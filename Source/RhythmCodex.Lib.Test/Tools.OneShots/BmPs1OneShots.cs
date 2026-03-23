using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Archs.Psx.Converters;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.Archs.Psx.Streamers;
using RhythmCodex.Charts.Bms.Converters;
using RhythmCodex.Charts.Models;
using RhythmCodex.FileSystems.Cd.Streamers;
using RhythmCodex.FileSystems.Cue.Processors;
using RhythmCodex.FileSystems.Cue.Streamers;
using RhythmCodex.FileSystems.Iso.Converters;
using RhythmCodex.Infrastructure;
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
    /// <summary>
    /// Renders a BMS set from each song on a PS1 beatmania disc.
    /// </summary>
    [Test]
    [TestCase("/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bm-eu.cue", "ps1-bm-eu")]
    [TestCase("/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bm-jp-disc1.cue", "ps1-bm-jp-disc1")]
    [TestCase("/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bm-jp-disc2.cue", "ps1-bm-jp-disc2")]
    [TestCase("/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bm3rd.cue", "ps1-bm3rd")]
    [TestCase("/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bm3rd-mini.cue", "ps1-bm3rd-mini")]
    [TestCase("/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bm4th.cue", "ps1-bm4th")]
    [TestCase("/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bm5th.cue", "ps1-bm5th")]
    [TestCase("/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bm6th.cue", "ps1-bm6th")]
    [TestCase("/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bmbest.cue", "ps1-bmbest")]
    [TestCase("/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bmclub.cue", "ps1-bmclub")]
    [TestCase("/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bmdct.cue", "ps1-bmdct")]
    [TestCase("/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bmgotta.cue", "ps1-bmgotta")]
    [TestCase("/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bmgotta2.cue", "ps1-bmgotta2")]
    [TestCase("/Volumes/RidgeportHDD/User Data/Bemani/Playstation/bmsot.cue", "ps1-bmsot")]
    [Explicit]
    public void ExtractBms(string source, string target)
    {
        const bool extractKeysounds = true;
        const bool extractCharts = true;
        const bool extractRawBlock = false;
        const bool extractBgm = true;

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
        var psxBeatmaniaDecoder = Resolve<IPsxBeatmaniaDecoder>();
        var psxBeatmaniaSongGrouper = Resolve<IPsxBeatmaniaSongGrouper>();

        //
        // Determine where SYSDATA.PAK is located on the disc and load it.
        //

        Log.WriteLine("Loading SYSDATA.PAK");
        using var sysDataPak = cdFiles.Single(f => f.Name == "./SYSDATA.PAK;1").Open();

        //
        // Decode SYSDATA.PAK.
        //

        var sysDataPakFiles = psxBeatmaniaDecoder.DecodeSysData(sysDataPak, sysDataPak.Length);

        //
        // Determine where BMDATA.PAK is located on the disc and load it.
        //

        Log.WriteLine("Loading BMDATA.PAK");
        using var bmDataPak = cdFiles.Single(f => f.Name == "./BMDATA.PAK;1").Open();

        //
        // Decode BMDATA.PAK.
        //

        var psxMgsSoundBankBlockReader = Resolve<IPsxMgsSoundBankBlockReader>();
        var psxMgsSoundTableBlockReader = Resolve<IPsxMgsSoundTableReader>();
        var mgsDecoder = Resolve<IPsxMgsDecoder>();
        var bmDataPakFiles = psxBeatmaniaDecoder.DecodeBmData(bmDataPak, bmDataPak.Length);
        var bmDataGroups = psxBeatmaniaSongGrouper.GroupFiles(bmDataPakFiles);

        foreach (var songGroup in bmDataGroups)
        {
            var groupPath = Path.Combine(target, $"{songGroup.Index:d4}");

            //
            // Convert charts.
            //

            var bmDataCharts = songGroup.Files
                .Where(f => f.Type == PsxBeatmaniaFileType.Chart)
                .Select((f, i) =>
                {
                    using var chartStream = new ReadOnlyMemoryStream(f.Data);
                    var chart = psxBeatmaniaDecoder.DecodeChart(chartStream);
                    chart[NumericData.Id] = i;
                    chart[NumericData.SourceIndex] = f.Index;

                    // Start of BGM is not explicitly included in the chart file,
                    // so it is added here.

                    chart.Events.Add(new Event
                    {
                        [NumericData.LinearOffset] = BigRational.Zero,
                        [NumericData.MetricOffset] = BigRational.Zero,
                        [NumericData.PlaySound] = 1
                    });

                    return chart;
                })
                .ToList();

            //
            // Convert keysound folders.
            //

            var soundBankBlocks = songGroup.Files
                .Where(x => x.Type == PsxBeatmaniaFileType.Keysound)
                .Select((x, i) => (
                    Index: i,
                    Bank: psxMgsSoundBankBlockReader.Read(new ReadOnlyMemoryStream(x.Data))
                ))
                .ToList();

            var soundTableMap = songGroup.Files
                .Where(x => x.Type == PsxBeatmaniaFileType.Kst)
                .Select((x, i) => (
                    Index: i,
                    Map: x.Index,
                    Table: psxMgsSoundTableBlockReader.Read(new ReadOnlyMemoryStream(x.Data)),
                    soundBankBlocks[i % soundBankBlocks.Count].Bank
                ))
                .ToList();

            var bmDataKeysoundSets = soundTableMap
                .Select(f =>
                {
                    var sounds = mgsDecoder.DecodeSounds(f.Bank, f.Table, 44100);

                    foreach (var sound in sounds)
                        sound[NumericData.SampleMap] = f.Index;

                    return (
                        f.Index,
                        f.Map,
                        Sounds: sounds
                    );
                })
                .Where(s => s.Sounds.Count > 0)
                .ToList();

            var writeSetConfig = new TestHelper.WriteSetConfig
            {
                OutPath = groupPath,
                Charts = bmDataCharts.ToList(),
                ChartSetId = songGroup.Index,
                Sounds = bmDataKeysoundSets.SelectMany(s => s.Sounds).ToList(),
                ChartType = BmsChartType.Beatmania,
                WriteCharts = extractCharts,
                WriteSounds = extractKeysounds,
                RemoveMissingSounds = false
            };

            this.WriteSet(writeSetConfig);
        }

        if (extractRawBlock)
        {
            //
            // Extract raw files.
            //

            foreach (var file in bmDataPakFiles)
            {
                var extension = file.Type switch
                {
                    PsxBeatmaniaFileType.Chart => "cs5",
                    PsxBeatmaniaFileType.Keysound => "ksb",
                    PsxBeatmaniaFileType.Kst => "kst",
                    PsxBeatmaniaFileType.Dat3 => "dat3",
                    _ => "bin"
                };

                this.WriteFile(file.Data.Span, Path.Combine(target, $"{file.Index:d4}.{extension}"));
            }
        }

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

        if (extractBgm)
        {
            List<XaChunk> xaChunks = [];

            var cdSectorsFactory = Resolve<ICdSectorCollectionFactory>();
            var isoInfoDecoder = Resolve<IIsoSectorInfoDecoder>();
            var streamFinder = Resolve<IXaCdStreamFinder>();

            switch (mchMode)
            {
                case 2:
                {
                    var mchDataReader = cdSectorsFactory.Create(mchDataPak, mchDataPak.Length);
                    var mchDataSectors = mchDataReader.Select(isoInfoDecoder.Decode);

                    xaChunks.AddRange(streamFinder.FindMode2(mchDataSectors, true));
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
                    File.WriteAllBytes(Path.Combine(outfolder, $"XA{xa.SourceIndex:00}{xa.SourceChannel:00}.wav"),
                        outStream.ToArray());
                    index++;
                }
            }
        }
    }
}