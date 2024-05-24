using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Beatmania.Converters;
using RhythmCodex.Beatmania.Streamers;
using RhythmCodex.Infrastructure;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;

namespace RhythmCodex.Beatmania.Integration;

[TestFixture]
public class BeatmaniaPcKeysoundIntegrationTests : BaseIntegrationFixture
{
    [Test]
    [Explicit]
    public void Test_Keysounds_9th()
    {
        var data = GetArchiveResource($"Beatmania.2dx9th.zip")
            .First()
            .Value;

        var decrypter = Resolve<IEncryptedBeatmaniaPcAudioStreamReader>();
        var streamer = Resolve<IBeatmaniaPcAudioStreamReader>();
        var decoder = Resolve<IBeatmaniaPcAudioDecoder>();
        var encoder = Resolve<IRiffPcm16SoundEncoder>();
        var writer = Resolve<IRiffStreamWriter>();

        var outFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "bmpc");
        if (!Directory.Exists(outFolder))
            Directory.CreateDirectory(outFolder);

        var decrypted = decrypter.Decrypt(new MemoryStream(data), data.Length);
        this.WriteFile(decrypted, Path.Combine(outFolder, "decrypted.2dx"));

        using var dataStream = new ReadOnlyMemoryStream(decrypted);
        var keysounds = streamer.Read(dataStream, dataStream.Length);
        var index = 1;
                
        foreach (var keysound in keysounds)
        {
            var decoded = decoder.Decode(keysound);
            var encoded = encoder.Encode(decoded);
            using var outStream = new MemoryStream();
            writer.Write(outStream, encoded);
            outStream.Flush();
            File.WriteAllBytes(Path.Combine(outFolder, $"{index:D4}.wav"), outStream.ToArray());
            index++;
        }
    }
}