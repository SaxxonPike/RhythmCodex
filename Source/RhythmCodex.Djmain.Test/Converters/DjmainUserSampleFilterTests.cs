using System.Collections.Generic;
using NUnit.Framework;
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
        public void Test1(int command, bool expectedInclusion)
        {
            // Arrange.
            var inputSamples = new Dictionary<int, DjmainSampleInfo>
            {
                { 1, new DjmainSampleInfo() },
                { 2, new DjmainSampleInfo() }
            };

            var inputEvents = new List<DjmainChartEvent>
            {
                new DjmainChartEvent { Offset = 0x1234, Param0 = (byte)command, Param1 = Create<byte>() }
            };

            // Act.
            var result = Subject.Filter(inputSamples, inputEvents);

            // Assert.
            
        }
    }
}
