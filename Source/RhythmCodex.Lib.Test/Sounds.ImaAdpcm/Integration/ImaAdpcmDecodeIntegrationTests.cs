using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.ImaAdpcm.Converters;
using RhythmCodex.Sounds.ImaAdpcm.Models;
using RhythmCodex.Sounds.Riff.Converters;
using RhythmCodex.Sounds.Riff.Streamers;

namespace RhythmCodex.Sounds.ImaAdpcm.Integration;

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