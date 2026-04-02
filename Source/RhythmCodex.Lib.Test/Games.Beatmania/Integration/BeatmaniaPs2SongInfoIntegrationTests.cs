using System.Linq;
using NUnit.Framework;
using RhythmCodex.Games.Beatmania.Ps2.Converters;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using Shouldly;

namespace RhythmCodex.Games.Beatmania.Integration;

[TestFixture]
public class BeatmaniaPs2SongInfoIntegrationTests : BaseIntegrationFixture
{
    [Test]
    public void Test_DecodeBeatmaniaUsSongInfoTable()
    {
        var data = GetArchiveResource("BeatmaniaPs2.bm2dxus-songlist.zip")
            .First()
            .Value;

        var formatDb = Resolve<IBeatmaniaPs2FormatDatabase>();
        var format = formatDb.GetFormatByType(BeatmaniaPs2FormatType.US);

        var songInfoDecoder = Resolve<IBeatmaniaPs2SongInfoDecoder>();
        var observed = songInfoDecoder
            .Decode(data, (int)format!.Value.MetaTables[0].SongTableOffset, BeatmaniaPs2FormatType.US);

        // Also tests to make sure that CP932 is used for decoding.
        observed.First().Name.ShouldBe("01 501_20，November");
    }
}