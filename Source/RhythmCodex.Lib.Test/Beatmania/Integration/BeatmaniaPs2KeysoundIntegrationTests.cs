using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Beatmania.Converters;
using RhythmCodex.Beatmania.Streamers;
using RhythmCodex.Dsp;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Sounds.Converters;

namespace RhythmCodex.Beatmania.Integration
{
    [TestFixture]
    public class BeatmaniaPs2KeysoundIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [Explicit]
        public void Test_Bgm_New()
        {
            var data = GetArchiveResource($"BeatmaniaPs2.bm2dxps2newbgm.zip")
                .First()
                .Value;

            var streamer = Resolve<IBeatmaniaPs2NewBgmStreamReader>();
            var decoder = Resolve<IBeatmaniaPs2BgmDecoder>();
            var dsp = Resolve<IAudioDsp>();
            var encoder = Resolve<IRiffPcm16SoundEncoder>();
            var writer = Resolve<IRiffStreamWriter>();

            var outFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            if (!Directory.Exists(outFolder))
                Directory.CreateDirectory(outFolder);

            using (var dataStream = new MemoryStream(data))
            {
                var bgm = streamer.Read(dataStream);

                var decoded = decoder.Decode(bgm);
                var processed = dsp.ApplyEffects(decoded);
                var encoded = encoder.Encode(processed);
                using (var outStream = new MemoryStream())
                {
                    writer.Write(outStream, encoded);
                    outStream.Flush();
                    File.WriteAllBytes(Path.Combine(outFolder, $"bgm.wav"), outStream.ToArray());
                }
            }
        }

        [Test]
        [Explicit]
        public void Test_Keys_New()
        {
            var data = GetArchiveResource($"BeatmaniaPs2.bm2dxps2newkey.zip")
                .First()
                .Value;

            var streamer = Resolve<IBeatmaniaPs2NewKeysoundStreamReader>();
            var decoder = Resolve<IBeatmaniaPs2KeysoundDecoder>();
            var dsp = Resolve<IAudioDsp>();
            var encoder = Resolve<IRiffPcm16SoundEncoder>();
            var writer = Resolve<IRiffStreamWriter>();

            var outFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "bmps2");
            if (!Directory.Exists(outFolder))
                Directory.CreateDirectory(outFolder);

            using (var dataStream = new MemoryStream(data))
            {
                var keysounds = streamer.Read(dataStream);

                foreach (var keysound in keysounds.Keysounds)
                {
                    var decoded = decoder.Decode(keysound);
                    var processed = dsp.ApplyEffects(decoded);
                    var encoded = encoder.Encode(processed);
                    using (var outStream = new MemoryStream())
                    {
                        writer.Write(outStream, encoded);
                        outStream.Flush();
                        File.WriteAllBytes(Path.Combine(outFolder, $"{keysound.SampleNumber:D4}.wav"),
                            outStream.ToArray());
                    }
                }
            }
        }

        [Test]
        [Explicit]
        public void Test_Bgm_Old()
        {
            var data = GetArchiveResource($"BeatmaniaPs2.bm2dxps2bgm.zip")
                .First()
                .Value;

            var streamer = Resolve<IBeatmaniaPs2OldBgmStreamReader>();
            var decoder = Resolve<IBeatmaniaPs2BgmDecoder>();
            var dsp = Resolve<IAudioDsp>();
            var encoder = Resolve<IRiffPcm16SoundEncoder>();
            var writer = Resolve<IRiffStreamWriter>();

            var outFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            if (!Directory.Exists(outFolder))
                Directory.CreateDirectory(outFolder);

            using (var dataStream = new MemoryStream(data))
            {
                var bgm = streamer.Read(dataStream);

                var decoded = decoder.Decode(bgm);
                var processed = dsp.ApplyEffects(decoded);
                var encoded = encoder.Encode(processed);
                using (var outStream = new MemoryStream())
                {
                    writer.Write(outStream, encoded);
                    outStream.Flush();
                    File.WriteAllBytes(Path.Combine(outFolder, $"bgm.wav"), outStream.ToArray());
                }
            }
        }

        [Test]
        [Explicit]
        public void Test_Keys_Old()
        {
            var data = GetArchiveResource($"BeatmaniaPs2.bm2dxps2key.zip")
                .First()
                .Value;

            var streamer = Resolve<IBeatmaniaPs2OldKeysoundStreamReader>();
            var decoder = Resolve<IBeatmaniaPs2KeysoundDecoder>();
            var dsp = Resolve<IAudioDsp>();
            var encoder = Resolve<IRiffPcm16SoundEncoder>();
            var writer = Resolve<IRiffStreamWriter>();

            var outFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "bmps2");
            if (!Directory.Exists(outFolder))
                Directory.CreateDirectory(outFolder);

            using (var dataStream = new MemoryStream(data))
            {
                var keysounds = streamer.Read(dataStream);

                foreach (var keysound in keysounds.Keysounds)
                {
                    var decoded = decoder.Decode(keysound);
                    var processed = dsp.ApplyEffects(decoded);
                    var encoded = encoder.Encode(processed);
                    using (var outStream = new MemoryStream())
                    {
                        writer.Write(outStream, encoded);
                        outStream.Flush();
                        File.WriteAllBytes(Path.Combine(outFolder, $"{keysound.SampleNumber:D4}.wav"),
                            outStream.ToArray());
                    }
                }
            }
        }
    }
}