using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
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
using RhythmCodex.Sounds.Converters;
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
        const bool extractCharts = false;
        const bool extractRawBlock = true;
        const bool extractBgm = false;
        const float keyVolume = 0.9f;
        const float xaVolume = 0.7f;

        var audioDsp = Resolve<IAudioDsp>();
        
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
                    var decodedSounds = mgsDecoder.DecodeSounds(f.Bank, f.Table, 44100);
                    var sounds = decodedSounds.Select(s => s.Sound).ToList();
                    var soundLog = decodedSounds.ToDictionary(
                        s => s.Index,
                        s => s.Packets
                            .ToDictionary(
                                pl => pl.Key,
                                pl => pl.Value
                                    .Select(p => $"{p.Data1:X2}{p.Data2:X2}{p.Data3:X2}{p.Data4:X2} " +
                                                 $"({p.Data1:d3},{p.Data2:d3},{p.Data3:d3},{p.Data4:d3}) " +
                                                 $"[{(p.Data1 >= 0x80 ? p.Command.ToString() : $"NoteOn({p.Data1})")}]")
                                    .ToList())
                    );

                    foreach (var sound in sounds)
                        sound[NumericData.SampleMap] = f.Index;

                    return (
                        f.Index,
                        f.Map,
                        Sounds: sounds,
                        Log: soundLog
                    );
                })
                .Where(s => s.Sounds.Count > 0)
                .ToList();

            //
            // Convert charts.
            //

            var sampleMaps = bmDataKeysoundSets
                .SelectMany(s => s.Sounds)
                .Select(s => (int?)s[NumericData.SampleMap] ?? 0)
                .ToHashSet();

            sampleMaps.Add(0);

            var chartFiles = songGroup.Files
                .Where(f => f.Type == PsxBeatmaniaFileType.Chart)
                .ToList();

            var chartsPerSampleMap = chartFiles.Count / sampleMaps.Count;

            var bmDataCharts = chartFiles
                .Select((f, i) =>
                {
                    using var chartStream = new ReadOnlyMemoryStream(f.Data);
                    var chart = psxBeatmaniaDecoder.DecodeChart(chartStream);
                    chart[NumericData.Id] = i;
                    chart[NumericData.SourceIndex] = f.Index;
                    chart[StringData.Title] = $"{f.Index:d4}";

                    var players = chart.Events
                        .Select(e => (int?)e[NumericData.Player] ?? 0)
                        .ToHashSet();

                    players.Add(0);

                    var chartSampleMap = players.Max() / chartsPerSampleMap;

                    // Start of BGM is not explicitly included in the chart file,
                    // so it is added here.

                    chart.Events.Add(new Event
                    {
                        [NumericData.LinearOffset] = BigRational.Zero,
                        [NumericData.MetricOffset] = BigRational.Zero,
                        [NumericData.PlaySound] = 1
                    });

                    chart[NumericData.SampleMap] = chartSampleMap;

                    return chart;
                })
                .ToList();

            //
            // Generate logs.
            //

            var keysoundLogSettings = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            foreach (var bmDataKeysoundSet in bmDataKeysoundSets)
            {
                var soundLogJson = JsonSerializer.Serialize(bmDataKeysoundSet.Log, keysoundLogSettings);
                this.WriteText(soundLogJson,
                    Path.Combine(groupPath, $"{bmDataKeysoundSet.Map:d4}{bmDataKeysoundSet.Index:d2}.log"));
            }

            var writeSetConfig = new TestHelper.WriteSetConfig
            {
                OutPath = groupPath,
                Charts = bmDataCharts.ToList(),
                ChartSetId = songGroup.Index,
                Sounds = bmDataKeysoundSets.SelectMany(s => s.Sounds).ToList(),
                ChartType = BmsChartType.Beatmania,
                WriteCharts = extractCharts,
                WriteSounds = extractKeysounds,
                RemoveMissingSounds = false,
                DeduplicateSounds = false,
                KeysoundVolume = keyVolume
            };

            var writeSetResult = this.WriteSet(writeSetConfig);
            
            var remappedSamplesJson = JsonSerializer.Serialize(writeSetResult.RemappedSamples, keysoundLogSettings);
            if (remappedSamplesJson.Length > 2)
                this.WriteText(remappedSamplesJson, Path.Combine(groupPath, $"remapped.log"));
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
                    PsxBeatmaniaFileType.Graphics => "gfx",
                    PsxBeatmaniaFileType.Script => "vfx",
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
                    sound[NumericData.Volume] = xaVolume;

                    var dspProcessed = audioDsp.ApplyEffects(sound);
                    var encoded = encoder.Encode(dspProcessed);

                    if (!Directory.Exists(outfolder))
                        Directory.CreateDirectory(outfolder);

                    using var outStream = new MemoryStream();
                    writer.Write(outStream, encoded);
                    outStream.Flush();
                    File.WriteAllBytes(Path.Combine(outfolder, $"XA{xa.SourceSector:d6}.wav"),
                        outStream.ToArray());
                    index++;
                }
            }
        }
    }
}