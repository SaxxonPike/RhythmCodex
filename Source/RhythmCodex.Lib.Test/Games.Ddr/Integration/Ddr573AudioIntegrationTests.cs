using System;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Archs.S573.Converters;
using RhythmCodex.Archs.S573.Providers;
using Shouldly;

namespace RhythmCodex.Games.Ddr.Integration;

[TestFixture]
public class Ddr573AudioIntegrationTests : BaseIntegrationFixture
{
    [Test]
    public void DecryptNewTest()
    {
        var inputArchive = GetArchiveResource("Ddr.mp3.zip");
        var data = inputArchive
            .First(name => name.Key.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
            .Value;
        var expected = inputArchive
            .First(name => name.Key.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
            .Value;
        var decrypter = Resolve<IDigital573AudioDecrypter>();
        var keyProvider = Resolve<IDigital573AudioKeyProvider>();
        var key = keyProvider.Get(data);
        var observed = decrypter.DecryptNew(data, key!);
        observed.Data.ToArray().ShouldBeEquivalentTo(expected);
    }

    [Test]
    [TestCase("sbm1")]
    [TestCase("sbm2")]
    public void DecryptOldTest(string archiveName)
    {
        var inputArchive = GetArchiveResource($"Ddr.{archiveName}.zip");
        var data = inputArchive
            .First(name => name.Key.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
            .Value;
        var expected = inputArchive
            .First(name => name.Key.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
            .Value;
        var decrypter = Resolve<IDigital573AudioDecrypter>();
        var keyProvider = Resolve<IDigital573AudioKeyProvider>();
        var key = keyProvider.Get(data);
        var observed = decrypter.DecryptOld(data, key!.Values[0]);
        observed.Data.ToArray().ShouldBeEquivalentTo(expected);
    }
}