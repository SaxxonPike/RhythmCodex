using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Compression;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ddr.Integration
{
    public class Ddr573SongDatabaseIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        public void Test1()
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
            var decoder = Resolve<IDdr573DatabaseDecoder>();

            var decrypted = decrypter.Decrypt(data, "EXTREME");
            var decompressed = decompressor.Decode(new MemoryStream(decrypted));
            var decoded = decoder.Decode(decompressed);

            decompressed.Should().BeEquivalentTo(expected);
        }
    }
}