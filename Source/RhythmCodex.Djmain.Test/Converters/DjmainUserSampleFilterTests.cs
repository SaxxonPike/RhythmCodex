using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters
{
    public class DjmainUserSampleFilterTests : BaseUnitTestFixture<DjmainUsedSampleFilter>
    {
        [TestCase(0x0, false)]
        [TestCase(0x1, true)]
        [TestCase(0x2, false)]
        [TestCase(0x3, false)]
        [TestCase(0x4, false)]
        [TestCase(0x5, true)]
        [TestCase(0x6, false)]
        [TestCase(0x7, false)]
        [TestCase(0x8, false)]
        [TestCase(0x9, false)]
        [TestCase(0xA, false)]
        [TestCase(0xB, false)]
        [TestCase(0xC, false)]
        [TestCase(0xD, false)]
        [TestCase(0xE, false)]
        [TestCase(0xF, false)]
        public void Filter_ShouldAcceptOnlyEventsWithSounds(int command, bool expectedInclusion)
        {
            // Arrange.
            var allIds = CreateMany<byte>();
            var expectedSample = Create<DjmainSampleInfo>();
            command |= Create<int>() & 0xF0;

            var inputSamples = new Dictionary<int, DjmainSampleInfo>
            {
                {allIds[0], expectedSample},
                {allIds[1], Create<DjmainSampleInfo>()}
            };

            var inputEvents = new[]
            {
                Build<DjmainChartEvent>()
                    .With(x => x.Param0, (byte)command)
                    .With(x => x.Param1, allIds.First())
                    .Create()
            };

            // Act.
            var result = Subject.Filter(inputSamples, inputEvents);

            // Assert.
            if (expectedInclusion)
            {
                result.Should()
                    .HaveCount(1)
                    .And.Contain(new KeyValuePair<int, DjmainSampleInfo>(allIds[0], expectedSample));
            }
            else
            {
                result.Should()
                    .BeEmpty();
            }
        }
    }
}
