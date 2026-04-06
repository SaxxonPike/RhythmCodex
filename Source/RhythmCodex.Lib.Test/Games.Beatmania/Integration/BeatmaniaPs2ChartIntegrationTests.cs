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
    public void Test_Blowfish()
    {
        var archive = GetArchiveResource("BeatmaniaPs2.bm2dxps2encchart.zip");
        var input = archive["01C ZERO.raw"];
        var expected = archive["01U ZERO.raw"];

        var keyProvider = Resolve<IBeatmaniaPs2KeyProvider>();
        var key = keyProvider.GetKeyFor14thStyle();
        var blowfish = Resolve<IBlowfishDecrypter>();

        var observed = blowfish.Decrypt(input, key).ToArray();
        observed.ShouldBeEquivalentTo(expected);
    }

    [Test]
    public void Test_LoadNewChart()
    {
        var archive = GetArchiveResource("BeatmaniaPs2.bm2dxps2newchart.zip");
        var input = archive.Single().Value;
        var inputMem = new MemoryStream(input);
        var newChartReader = Resolve<IBeatmaniaPs2NewChartStreamReader>();
        var newChartDecoder = Resolve<IBeatmaniaPs2NewChartDecoder>();

        var observed = newChartDecoder.Decode(newChartReader.Read(inputMem, inputMem.Length).Span);

        observed.NoteCounts.ShouldBeEquivalentTo(new Dictionary<int, int>
        {
            { 0, 323 }
        });

        observed.Rate.ShouldBeEquivalentTo(new BigRational(16683 * 2 - 1, 1000000 * 2));
    }

    [Test]
    public void Test_ConvertNewChart()
    {
        var archive = GetArchiveResource("BeatmaniaPs2.bm2dxps2newchart.zip");
        var input = archive.Single().Value;
        var inputMem = new MemoryStream(input);
        var newChartReader = Resolve<IBeatmaniaPs2NewChartStreamReader>();
        var newChartDecoder = Resolve<IBeatmaniaPs2NewChartDecoder>();
        var decoder = Resolve<IBeatmaniaPs2ChartConverter>();
        
        var chart = newChartDecoder.Decode(newChartReader.Read(inputMem, inputMem.Length).Span);
        var decoded = decoder.Convert(chart);

        decoded.Events.Count.ShouldBe(444);
    }
}