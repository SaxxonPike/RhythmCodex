using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.ImaAdpcm.Converters;
using RhythmCodex.ImaAdpcm.Models;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;

namespace RhythmCodex.ImaAdpcm.Integration;

[TestFixture]
[Explicit]
public class ImaAdpcmDecodeIntegrationTests : BaseIntegrationFixture
{
    [Test]
    public void TestXboxAdpcm()
    {
        var decoder = Resolve<IImaAdpcmDecoder>();
        var encoder = Resolve<IRiffPcm16SoundEncoder>();
        var writer = Resolve<IRiffStreamWriter>();
            
        var data = GetArchiveResource("ImaAdpcm.RawXboxAdpcm.zip")
            .First()
            .Value;

        var ima = new ImaAdpcmChunk
        {
            Channels = 2,
            ChannelSamplesPerFrame = 64,
            Data = data,
            Rate = 44100
        };

        var sound = decoder.Decode(ima)!;
        const int index = 0;
            
        sound[NumericData.Rate] = ima.Rate;
        var encoded = encoder.Encode(sound);
        var outfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ima");
        if (!Directory.Exists(outfolder))
            Directory.CreateDirectory(outfolder);

        using var outStream = new MemoryStream();
        writer.Write(outStream, encoded);
        outStream.Flush();
        File.WriteAllBytes(Path.Combine(outfolder, $"{index:000}.wav"), outStream.ToArray());
    }
}