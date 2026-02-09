using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Archs.Djmain.Converters;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Charts.Bms.Converters;
using RhythmCodex.Charts.Bms.Streamers;
using RhythmCodex.Extensions;
using RhythmCodex.Games.Beatmania.Psx.Streamers;
using RhythmCodex.Infrastructure;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Riff.Converters;
using RhythmCodex.Sounds.Riff.Streamers;
using RhythmCodex.Sounds.Vag.Converters;

namespace RhythmCodex.Games.Beatmania.Psx.Integration;

[TestFixture]
public class BeatmaniaPsxBmDataStreamReaderIntegrationTests : BaseIntegrationFixture
{
    [Test]
    [Explicit]
    [TestCase(@"E:\BMDATA.PAK")]
    public void Test1(string fileName)
    {
        var reader = Resolve<IBeatmaniaPsxBmDataStreamReader>();
        var chartReader = Resolve<IBeatmaniaPsxChartEventStreamReader>();
        var chartDecoder = Resolve<IDjmainChartDecoder>();
        var chartEncoder = Resolve<IBmsEncoder>();
        var chartWriter = Resolve<IBmsStreamWriter>();
        var keysoundReader = Resolve<IBeatmaniaPsxKeysoundStreamReader>();
        var keysoundDecoder = Resolve<IVagDecoder>();
        var keysoundEncoder = Resolve<IRiffPcm16SoundEncoder>();
        var keysoundWriter = Resolve<IRiffStreamWriter>();

        using var file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        var output = reader.Read(file, (int) file.Length);

        var outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "bmdata");
        var folderIndex = 0;
        foreach (var folder in output)
        {
            var fileIndex = 0;
            Directory.CreateDirectory(Path.Combine(outputFolder, $"{folderIndex:X4}"));
            foreach (var folderFile in folder.Files)
            {
                var data = folderFile.Data;
                var extension = "bin";

                if (data.Length >= 4 && data[^4..].ToArray()
                        .SequenceEqual(new byte[] {0xFF, 0x7F, 0x00, 0x00}))
                {
                    extension = "cs5";
                    using var chartStream = new MemoryStream();
                    var chart = chartDecoder.Decode(chartReader.Read(new ReadOnlyMemoryStream(data), data.Length), DjmainChartType.Beatmania, true);
                    chart.PopulateMetricOffsets();
                    chartWriter.Write(chartStream, chartEncoder.Encode(chart));
                    File.WriteAllBytes(Path.Combine(outputFolder, $"{folderIndex:X4}", $"{fileIndex:X4}.bme"), chartStream.ToArray());
                }

                else if (data.Length >= 4 && data[^4..].ToArray().SequenceEqual(new byte[] {0x77, 0x77, 0x77, 0x77}))
                {
                    var keyOutFolder = Path.Combine(outputFolder, $"{folderIndex:X4}", $"key_{fileIndex:X4}");
                    if (!Directory.Exists(keyOutFolder))
                        Directory.CreateDirectory(keyOutFolder);

                    using var keyStream = new ReadOnlyMemoryStream(data);
                    var keysounds = keysoundReader.Read(keyStream);
                    var keyIndex = 1;
                
                    foreach (var keysound in keysounds)
                    {
                    
                        var decoded = keysoundDecoder.Decode(keysound.Data);
                        decoded[NumericData.Rate] = 32000;
                        var encoded = keysoundEncoder.Encode(decoded);
                        using var outStream = new MemoryStream();
                        keysoundWriter.Write(outStream, encoded);
                        outStream.Flush();
                        File.WriteAllBytes(Path.Combine(keyOutFolder, $"{Alphabet.EncodeAlphanumeric(keyIndex, 2)}.wav"), outStream.ToArray());
                        keyIndex++;
                    }
                }
                File.WriteAllBytes(Path.Combine(outputFolder, $"{folderIndex:X4}", $"{fileIndex:X4}.{extension}"), folderFile.Data.ToArray());
                fileIndex++;
            }

            folderIndex++;
        }
    }
}