using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Charts.Step2.Converters;
using RhythmCodex.Charts.Step2.Streamers;
using RhythmCodex.Metadatas.Models;
using Shouldly;

namespace RhythmCodex.Charts.Step2.Integration;

[TestFixture]
public class Step2DecodeIntegrationTests : BaseIntegrationFixture
{
    [Test]
    [TestCase("single", 118, 0)]
    [TestCase("couple", 112, 112)]
    [TestCase("double", 47, 48)]
    public void Test1(string chartName, int expectedStepsP1, int expectedStepsP2)
    {
        var streamReader = Resolve<IStep2StreamReader>();
        var decoder = Resolve<IStep2Decoder>();
            
        var data = GetArchiveResource($"Step2.{chartName}.zip")
            .First()
            .Value;

        var chunk = streamReader.Read(new MemoryStream(data), data.Length);
        var chart = decoder.Decode(chunk);

        chart.Events.Count(ev => ev[FlagData.Note] == true && ev[NumericData.Player] == 0).ShouldBe(expectedStepsP1);
        chart.Events.Count(ev => ev[FlagData.Note] == true && ev[NumericData.Player] == 1).ShouldBe(expectedStepsP2);
    }
}