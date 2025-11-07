using System;
using System.IO;
using System.Linq;
using Shouldly;
using NUnit.Framework;
using RhythmCodex.Beatmania.Pc.Streamers;
using RhythmCodex.Extensions;

namespace RhythmCodex.Beatmania.Streamers;

[TestFixture]
public class EncryptedBeatmaniaPcAudioStreamReaderTests : BaseUnitTestFixture<EncryptedBeatmaniaPcAudioStreamReader,
    IEncryptedBeatmaniaPcAudioStreamReader>
{
    [Test]
    [TestCase("Beatmania.2dx9th.zip", "/u/home/local/us")]
    public void Decrypt_ShouldReturnCorrectData(string fileName, string expectedId)
    {
        // Only checks ID for now; ideally we would verify against a known good plaintext
        var archive = GetArchiveResource(fileName);
        var files = archive
            .Where(x => x.Key.EndsWith(".2dx", StringComparison.InvariantCultureIgnoreCase))
            .Select(x => x.Value)
            .ToList();
        
        foreach (var file in files)
        {
            using var stream = new MemoryStream(file);
            var decrypted = Subject.Decrypt(stream, file.Length);
            var observed = decrypted.Span[..0x10].NullTerminated().GetString();
            observed.ShouldBe(expectedId);
        }
    }
}