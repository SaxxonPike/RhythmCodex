using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Ogg.Converters;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;

namespace RhythmCodex.ThirdParty.Integration;

[TestFixture]
public class OggDecoderIntegrationTests : BaseIntegrationFixture
{
    [Test]
    [Explicit]
    public void Test_OGG()
    {
        var data = GetArchiveResource($"Ogg.example.ogg.zip")
            .First()
            .Value;
            
        var decoder = Resolve<IOggDecoder>();
        var encoder = Resolve<IRiffPcm16SoundEncoder>();
        var writer = Resolve<IRiffStreamWriter>();
            
        var decoded = decoder.Decode(new MemoryStream(data));
        var encoded = encoder.Encode(decoded);
        using var outStream = new MemoryStream();
        writer.Write(outStream, encoded);
        outStream.Flush();
        File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ogg.wav"), outStream.ToArray());
    }
}