using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using RhythmCodex.Charting.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;
using RhythmCodex.Stepmania;
using RhythmCodex.Stepmania.Converters;
using RhythmCodex.Stepmania.Model;
using RhythmCodex.Stepmania.Streamers;
using RhythmCodex.Vtddd.Converters;
using RhythmCodex.Vtddd.Streamers;

namespace RhythmCodex.Vtddd.Integration;

[TestFixture]
public class DecodeVtdddIntegrationTests : BaseIntegrationFixture
{
    [Test]
    [Explicit("under construction - add/modify a test case pointing to your own MUSIC folder")]
    [TestCase(@"C:\Users\Saxxon\Desktop\VTDDD\Dance", "VTDDD")]
    [TestCase(@"C:\Users\Saxxon\Desktop\VTDDD\Music", "VTDDD")]
    [TestCase(@"C:\Users\Saxxon\Desktop\VTDDD\Previews", "VTDDD")]
    [TestCase(@"C:\Program Files (x86)\Digital Praise\Dance Praise\Dance Praise\Resources\Dance\Music", "Dance Praise")]
    public void DecodeDpo(string inPath, string outFolder)
    {
        var dpoStreamReader = Resolve<IVtdddDpoStreamReader>();
        var outPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), outFolder);
        if (!Directory.Exists(outPath))
            Directory.CreateDirectory(outPath);

        var inFiles = Directory.GetFiles(inPath, "*.dpo");
        foreach (var inFile in inFiles)
        {
            using var fileStream = File.OpenRead(inFile);
            var dpo = dpoStreamReader.Read(fileStream, (int)fileStream.Length);
            var outFile = Path.Combine(outPath, $"{Path.GetFileName(inFile)}.ogg");
            File.WriteAllBytes(outFile, dpo.Data);
        }
    }

    [Test]
    [Explicit("under construction - modify test case pointing to your own SCRIPTS/DANCE.XML file")]
    [TestCase(@"C:\Users\Saxxon\Desktop\VTDDD\Scripts\dance.xml", "2062", "VTDDD")]
    [TestCase(@"C:\Program Files (x86)\Digital Praise\Dance Praise\Dance Praise\Resources\Scripts\dance.xml", "2062", "Dance Praise")]
    [TestCase(@"C:\Program Files (x86)\Digital Praise\Dance Praise\Dance Praise\Resources\Scripts\songs_1.xml", "2062", "Dance Praise")]
    [TestCase(@"C:\Program Files (x86)\Digital Praise\Dance Praise\Dance Praise\Resources\Scripts\songs_2.xml", "2062", "Dance Praise")]
    [TestCase(@"C:\Program Files (x86)\Digital Praise\Dance Praise\Dance Praise\Resources\Scripts\songs_4.xml", "2062", "Dance Praise")]
    [TestCase(@"C:\Program Files (x86)\Digital Praise\Dance Praise\Dance Praise\Resources\Scripts\songs_5.xml", "2062", "Dance Praise")]
    [TestCase(@"C:\Program Files (x86)\Digital Praise\Dance Praise\Dance Praise\Resources\Scripts\songs_6.xml", "2062", "Dance Praise")]
    [TestCase(@"C:\Program Files (x86)\Digital Praise\Dance Praise\Dance Praise\Resources\Scripts\songs_7.xml", "2062", "Dance Praise")]
    [TestCase(@"C:\Program Files (x86)\Digital Praise\Dance Praise\Dance Praise\Resources\Scripts\songs_8.xml", "2062", "Dance Praise")]
    [TestCase(@"C:\Program Files (x86)\Digital Praise\Dance Praise\Dance Praise\Resources\Scripts\songs_9.xml", "2062", "Dance Praise")]
    public void DecodeDanceXml(string inPath, string chartPrefix, string outFolder)
    {
        var danceXmlStreamReader = Resolve<IVtdddDanceXmlStreamReader>();
        var chartXmlStreamReader = Resolve<IVtdddChartXmlStreamReader>();
        var chartDecoder = Resolve<IVtdddChartDecoder>();
        var smEncoder = Resolve<ISmEncoder>();
        var smWriter = Resolve<ISmStreamWriter>();
        var outPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), outFolder);

        using var stream = File.Open(inPath, FileMode.Open, FileAccess.Read);
        var db = danceXmlStreamReader.Read(stream, chartPrefix);
        var scriptPath = Path.GetDirectoryName(inPath);
        foreach (var song in db.Tracks)
        {
            var charts = new List<Chart>();
            const int bpm = 145;
            var chartConfig = new[]
            {
                (song.ChartEasy, SmNotesDifficulties.Easy),
                (song.ChartMedium, SmNotesDifficulties.Medium),
                (song.ChartHard, SmNotesDifficulties.Hard),
                (song.ChartExpert, SmNotesDifficulties.Challenge)
            };

            foreach (var (chartFile, chartDifficulty) in chartConfig)
            {
                var chartPath = Path.Combine(scriptPath!, chartFile);
                if (!File.Exists(chartPath))
                    continue;

                using var chartStream = File.OpenRead(chartPath);
                var chart = chartDecoder.Decode(chartXmlStreamReader.Read(chartStream));
                chart.Events.Insert(0, new Event
                {
                    [NumericData.LinearOffset] = BigRational.Zero,
                    [NumericData.Bpm] = bpm
                });
                chart.PopulateMetricOffsets();
                chart.QuantizeMetricOffsets(96);
                chart[NotesCommandTag.DifficultyTag] = chartDifficulty;
                chart[NotesCommandTag.TypeTag] = SmGameTypes.DanceSingle;
                charts.Add(chart);
            }

            var metadata = new Metadata
            {
                [StringData.Title] = song.Title,
                [StringData.Artist] = song.Artist,
                [NumericData.Bpm] = bpm,
                [StringData.Music] = $"{song.Wave}.ogg",
                [ChartTag.DisplayBpmTag] = "*"
            };

            var chartSet = new ChartSet()
            {
                Charts = charts,
                Metadata = metadata
            };

            var encoded = smEncoder.Encode(chartSet);
            var finalPath = Path.Combine(Path.Combine(outPath, outFolder, $"{song.SongId + 1:D4}"));
            if (!Directory.Exists(finalPath))
                Directory.CreateDirectory(finalPath);
            var finalFile = Path.Combine(finalPath, $"{song.Wave}.sm");
                
            using var outStream = File.Open(finalFile, FileMode.Create, FileAccess.ReadWrite);
            smWriter.Write(outStream, encoded);
            outStream.Flush();
        }
    }
}