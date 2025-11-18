using System.IO;
using NUnit.Framework;
using RhythmCodex.Plugin.Sdl3.Sounds;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Integration;

[TestFixture]
public class SoundStreamReaderTests : BaseIntegrationFixture<SoundStreamReader>
{
    [Test]
    [TestCase("Flac.example.flac.zip")]
    [TestCase("Mp3.example.mp3.zip")]
    [TestCase("Ogg.example.ogg.zip")]
    [TestCase("Wav.double.zip")]
    [TestCase("Wav.imaadpcm.zip")]
    [TestCase("Wav.mp3.zip")]
    [TestCase("Wav.msadpcm.zip")]
    [TestCase("Wav.pcm8.zip")]
    [TestCase("Wav.pcm16.zip")]
    [TestCase("Wav.pcm24.zip")]
    [TestCase("Wav.pcm32.zip")]
    [TestCase("Wav.single.zip")]
    public void Test_ReadsFile(string testCase)
    {
        foreach (var (_, value) in GetArchiveResource(testCase))
        {
            using var mem = new MemoryStream(value);
            Sound? sound = null;
            Assert.DoesNotThrow(() => { Subject.Read(mem); });
            Log.WriteLine(sound);
        }
    }
}