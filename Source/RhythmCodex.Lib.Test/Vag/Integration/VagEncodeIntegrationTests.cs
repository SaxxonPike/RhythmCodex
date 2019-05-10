using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Infrastructure;
using RhythmCodex.Vag.Converters;
using RhythmCodex.Vag.Models;
using RhythmCodex.Wav.Converters;

namespace RhythmCodex.Vag.Integration
{
    [TestFixture]
    public class VagEncodeIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [TestCase("msadpcm")]
        [Explicit("WIP")]
        public void Test1(string name)
        {
            var decoder = Resolve<IWavDecoder>();
            var encoder = Resolve<IVagEncrypter>();
            
            var data = GetArchiveResource($"Wav.{name}.zip")
                .First()
                .Value;

            var sound = decoder.Decode(new ReadOnlyMemoryStream(data));
            var decoded = sound.Samples[0].Data.ToArray();
            var encoded = new byte[decoded.Length * 16 / 28];
            encoder.Encrypt(decoded, encoded, decoded.Length, new VagState());
            File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "out.vag"), encoded);
        }
    }
}