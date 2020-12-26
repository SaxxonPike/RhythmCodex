using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Step1.Heuristics
{
    public class Step1HeuristicTests : BaseUnitTestFixture<Step1Heuristic>
    {
        [Test]
        [TestCase("picky")]
        [TestCase("solo")]
        [TestCase("2nd")]
        public void Match_ShouldMatchStep1Files(string chartName)
        {
            var data = GetArchiveResource($"Step1.{chartName}.zip")
                .First()
                .Value;

            Subject.Match(data).Should().NotBeNull();
        }
    }
}