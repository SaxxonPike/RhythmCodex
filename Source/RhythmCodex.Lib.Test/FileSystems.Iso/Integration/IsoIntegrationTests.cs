using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.FileSystems.Iso.Converters;
using RhythmCodex.FileSystems.Iso.Streamers;
using Shouldly;

namespace RhythmCodex.FileSystems.Iso.Integration;

[TestFixture]
public class IsoIntegrationTests : BaseIntegrationFixture
{
    [Test]
    public void TestBoringIso()
    {
        var data = GetArchiveResource("Iso.test-iso.zip")
            .First()
            .Value;
        var mem = new MemoryStream(data);

        var reader = Resolve<IIsoSectorCollectionFactory>();
        var decoder = Resolve<IIsoCdFileDecoder>();

        var sectors = reader.Create(mem, mem.Length);
        var files = decoder.Decode(sectors);

        var foundNames = files.Select(x => x.Name!).ToList();
        var expectedNames = new List<string>
        {
            "./COMMAND.COM",
            "./FDISK.EXE",
            "./FORMAT.COM",
            "./IO.SYS",
            "./MSDOS.SYS",
            "./SYS.COM"
        };

        foundNames.ShouldBeEquivalentTo(expectedNames);
    }
}