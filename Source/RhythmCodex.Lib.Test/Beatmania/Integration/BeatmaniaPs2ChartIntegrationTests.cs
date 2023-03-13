using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Beatmania.Converters;
using RhythmCodex.ThirdParty;

namespace RhythmCodex.Beatmania.Integration;

[TestFixture]
public class BeatmaniaPs2ChartIntegrationTests : BaseIntegrationFixture
{
    [Test]
    [Ignore("WIP, does not currently work")]
    public void Test1()
    {
        var archive = GetArchiveResource("BeatmaniaPs2.bm2dxps2encchart.zip");
        var input = archive["01C ZERO.raw"];
        var expected = archive["01U ZERO.raw"];

        var key = Resolve<IBeatmaniaPs2KeyProvider>().GetKeyFor14thStyle();
        var blowfish = Resolve<IBlowfishDecrypter>();
        var observed = blowfish.Decrypt(input, key);

        observed.Should().BeEquivalentTo(expected);
    }
}