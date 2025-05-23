using System;
using System.IO;
using System.Linq;
using Shouldly;
using NUnit.Framework;
using RhythmCodex.Psf.Streamers;

namespace RhythmCodex.Psf.Integration;

public class PsfIntegrationTests : BaseIntegrationFixture
{
    [Test]
    [TestCase("ff9")]
    public void Read_ShouldReturnCorrectData(string name)
    {
        // Arrange.
        var source = GetArchiveResource($"Psf.{name}.zip")
            .First(f => f.Key.EndsWith(".psf", StringComparison.OrdinalIgnoreCase))
            .Value;
        var expected = GetArchiveResource($"Psf.{name}.zip")
            .First(f => f.Key.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
            .Value;

        var reader = Resolve<IPsfStreamReader>();
        var psf = reader.Read(new MemoryStream(source));
        psf.Reserved.ShouldBeEmpty();
        psf.Data.ShouldBeEquivalentTo(expected);
    }
}