using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Mp3.Converters;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;

namespace RhythmCodex.ThirdParty.Integration;

[TestFixture]
public class Mp3DecoderIntegrationTests : BaseIntegrationFixture
{
    [Test]
    [Explicit]
    public void Test_MP3()
    {
        var data = GetArchiveResource($"Mp3.example.mp3.zip")
            .First()
            .Value;
            
        var decoder = Resolve<IMp3Decoder>();
        var encoder = Resolve<IRiffPcm16SoundEncoder>();
        var writer = Resolve<IRiffStreamWriter>();
            
        var decoded = decoder.Decode(new MemoryStream(data));
        var encoded = encoder.Encode(decoded);
        using var outStream = new MemoryStream();
        writer.Write(outStream, encoded);
        outStream.Flush();
        File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "mp3.wav"), outStream.ToArray());
    }
}