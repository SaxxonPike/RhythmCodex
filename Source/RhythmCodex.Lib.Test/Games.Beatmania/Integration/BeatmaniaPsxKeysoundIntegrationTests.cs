using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Games.Beatmania.Psx.Streamers;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Riff.Converters;
using RhythmCodex.Sounds.Riff.Streamers;
using RhythmCodex.Sounds.Vag.Converters;

namespace RhythmCodex.Games.Beatmania.Integration;

[TestFixture]
public class BeatmaniaPsxKeysoundIntegrationTests : BaseIntegrationFixture
{
    [Test]
    [Explicit]
    public void Test_Keysounds()
    {
        var data = GetArchiveResource("BeatmaniaPsx.keysounds.zip")
            .First()
            .Value;

        var streamer = Resolve<IBeatmaniaPsxKeysoundStreamReader>();
        var decoder = Resolve<IVagDecoder>();
        var encoder = Resolve<IRiffPcm16SoundEncoder>();
        var writer = Resolve<IRiffStreamWriter>();

        var outFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "bmpsx");
        if (!Directory.Exists(outFolder))
            Directory.CreateDirectory(outFolder);

        using var dataStream = new MemoryStream(data);
        var keysounds = streamer.Read(dataStream);
                
        foreach (var keysound in keysounds)
        {
                    
            var decoded = decoder.Decode(keysound.Data);
            decoded[NumericData.Rate] = 32000;
            var encoded = encoder.Encode(decoded);
            using var outStream = new MemoryStream();
            writer.Write(outStream, encoded);
            outStream.Flush();
            File.WriteAllBytes(Path.Combine(outFolder, $"{keysound.DirectoryEntry.Offset:X6}.wav"), outStream.ToArray());
        }
    }
}