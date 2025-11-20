using NUnit.Framework;
using RhythmCodex.Encryptions.Blowfish.Converters;
using RhythmCodex.Games.Beatmania.Ps2.Converters;
using Shouldly;

namespace RhythmCodex.Games.Beatmania.Integration;

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

        observed.ShouldBeEquivalentTo(expected);
    }
}