using System;
using System.Linq;
using Shouldly;
using NUnit.Framework;
using RhythmCodex.Compression;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ddr.Integration;

public class Ddr573SongDatabaseIntegrationTests : BaseIntegrationFixture
{
    [Test]
    public void Decrypt_ShouldProduceExpectedResult()
    {
        var inputArchive = GetArchiveResource($"Ddr.mdb.zip");
        var data = inputArchive
            .First(name => name.Key.Equals("mdb", StringComparison.OrdinalIgnoreCase))
            .Value;
        var expected = inputArchive
            .First(name => name.Key.Equals("expected", StringComparison.OrdinalIgnoreCase))
            .Value;

        var decrypter = Resolve<IDdr573DatabaseDecrypter>();
        var decompressor = Resolve<IBemaniLzDecoder>();
        var decrypted = decrypter.Decrypt(data, decrypter.ConvertKey("EXTREME"));
        var decompressed = decompressor.Decode(new ReadOnlyMemoryStream(decrypted));
        decompressed.ToArray().ShouldBeEquivalentTo(expected);
    }

    [Test]
    public void FindKey_ShouldFindCorrectKey()
    {
        var inputArchive = GetArchiveResource($"Ddr.mdb.zip");
        var data = inputArchive
            .First(name => name.Key.Equals("mdb", StringComparison.OrdinalIgnoreCase))
            .Value;

        var decrypter = Resolve<IDdr573DatabaseDecrypter>();
        var observed = decrypter.FindKey(data);
        observed.ShouldBe(decrypter.ConvertKey("EXTREME"));
    }

    [Test]
    public void FindRecordSize_Test()
    {
        var inputArchive = GetArchiveResource($"Ddr.mdb.zip");
        var data = inputArchive
            .First(name => name.Key.Equals("expected", StringComparison.OrdinalIgnoreCase))
            .Value;

        var decoder = Resolve<IDdr573DatabaseDecoder>();
        var observed = decoder.FindRecordSize(data);
        observed.ShouldBe(0x80);
    }
}