using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Encryptions.Blowfish.Converters;
using RhythmCodex.Games.Beatmania.Ps2.Converters;
using RhythmCodex.Games.Beatmania.Ps2.Streamers;
using RhythmCodex.Infrastructure;
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

    [Test]
    public void Test_LoadNewChart()
    {
        var archive = GetArchiveResource("BeatmaniaPs2.bm2dxps2newchart.zip");
        var input = archive.Single().Value;
        var inputMem = new MemoryStream(input);
        var newChartReader = Resolve<IBeatmaniaPs2NewChartStreamReader>();

        var observed = newChartReader.Read(inputMem, inputMem.Length);

        observed.NoteCounts.ShouldBeEquivalentTo(new Dictionary<int, int>
        {
            { 0, 323 }
        });

        observed.Rate.ShouldBeEquivalentTo(new BigRational(16683, 1000000));
    }

    [Test]
    public void Test_ConvertNewChart()
    {
        var archive = GetArchiveResource("BeatmaniaPs2.bm2dxps2newchart.zip");
        var input = archive.Single().Value;
        var inputMem = new MemoryStream(input);
        var newChartReader = Resolve<IBeatmaniaPs2NewChartStreamReader>();
        var decoder = Resolve<IBeatmaniaPs2ChartDecoder>();
        
        var chart = newChartReader.Read(inputMem, inputMem.Length);
        var decoded = decoder.Decode(chart);

        decoded.Events.Count.ShouldBe(443);
    }
}