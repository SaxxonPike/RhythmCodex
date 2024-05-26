using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;
using RhythmCodex.Vag.Converters;
using RhythmCodex.Vag.Models;
using RhythmCodex.Wav.Converters;

namespace RhythmCodex.Vag.Integration;

[TestFixture]
public class VagEncodeIntegrationTests : BaseIntegrationFixture
{
    [Test]
    [TestCase("pcm16")]
    [Explicit("WIP")]
    public void Test1(string name)
    {
        var wavDecoder = Resolve<IWavDecoder>();
        var vagEncoder = Resolve<IVagEncrypter>();
            
        var data = GetArchiveResource($"Wav.{name}.zip")
            .First()
            .Value;

        var sound = wavDecoder.Decode(new ReadOnlyMemoryStream(data));
        var decoded = sound!.Samples[0].Data.ToArray();
        var encoded = new byte[decoded.Length * 16 / 28];
        vagEncoder.Encrypt(decoded, encoded, decoded.Length, new VagState());
        File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "out.vag"), encoded);

        var vagDecoder = Resolve<IVagDecoder>();
        var decodedVag = vagDecoder.Decode(new VagChunk
        {
            Channels = 1,
            Length = encoded.Length & ~0xF,
            Data = encoded,
            Interleave = 16
        });

        decodedVag![NumericData.Rate] = sound[NumericData.Rate];
        this.WriteSound(decodedVag, "out.vag.wav");
    }
}