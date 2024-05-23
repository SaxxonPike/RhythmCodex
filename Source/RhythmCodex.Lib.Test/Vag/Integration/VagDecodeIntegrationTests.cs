using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Vag.Converters;
using RhythmCodex.Vag.Streamers;

namespace RhythmCodex.Vag.Integration;

[TestFixture]
public class VagDecodeIntegrationTests : BaseIntegrationFixture
{
    [Test]
    [Explicit]
    public void Test_Xa2()
    {
        var data = GetArchiveResource($"Vag.xa2.zip")
            .First()
            .Value;

        var decoder = Resolve<IVagDecoder>();
        var streamer = Resolve<IXa2StreamReader>();

        using var stream = new MemoryStream(data);
        var vag = streamer.Read(stream);
        var decoded = decoder.Decode(vag.VagChunk);
        decoded[NumericData.Rate] = 44100;
        this.WriteSound(decoded, "xa2.wav");
    }

    [Test]
    [Explicit]
    public void Test_Svag()
    {
        var data = GetArchiveResource($"Vag.svag.zip")
            .First()
            .Value;

        var decoder = Resolve<IVagDecoder>();
        var streamer = Resolve<ISvagStreamReader>();
        var encoder = Resolve<IRiffPcm16SoundEncoder>();
        var writer = Resolve<IRiffStreamWriter>();

        using var stream = new MemoryStream(data);
        var vag = streamer.Read(stream);
        var decoded = decoder.Decode(vag.VagChunk);
        decoded[NumericData.Rate] = vag.SampleRate;
        var encoded = encoder.Encode(decoded);
        using var outStream = new MemoryStream();
        writer.Write(outStream, encoded);
        outStream.Flush();
        File.WriteAllBytes(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "svag.wav"),
            outStream.ToArray());
    }
}