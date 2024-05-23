using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Infrastructure;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Providers;
using RhythmCodex.Wav.Converters;

namespace RhythmCodex.Sounds.Integration;

[TestFixture]
public class ResamplerIntegrationTests : BaseIntegrationFixture
{
    [Test]
    [Explicit("writes to the desktop")]
    public void Test_AllResamplers()
    {
        var decoder = Resolve<IWavDecoder>();
        var encoder = Resolve<IRiffPcm16SoundEncoder>();
        var writer = Resolve<IRiffStreamWriter>();
        var provider = Resolve<IResamplerProvider>();
        var dsp = Resolve<IAudioDsp>();
        var resamplers = provider.Get();

        foreach (var resampler in resamplers)
        {
            var data = GetArchiveResource($"Wav.pcm16.zip")
                .First()
                .Value;

            var sound = decoder.Decode(new ReadOnlyMemoryStream(data));
            sound = dsp.ApplyResampling(sound, resampler, 44100);
            var encoded = encoder.Encode(sound);
            var outfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "wav");
            if (!Directory.Exists(outfolder))
                Directory.CreateDirectory(outfolder);

            using var outStream = new MemoryStream();
            writer.Write(outStream, encoded);
            outStream.Flush();
            File.WriteAllBytes(Path.Combine(outfolder, $"{resampler.Name}.wav"), outStream.ToArray());
        }
    }
}