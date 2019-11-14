using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Djmain.Converters
{
    [TestFixture]
    public class DjmainChartDecoderTests : BaseUnitTestFixture<DjmainChartDecoder, IDjmainChartDecoder>
    {
        [Test]
        public void Decode_SkipsDoublePlayNoteCount_WhenChartTypeIsBeatmania()
        {
            // Arrange.
            var offset = Create<ushort>();
            var data = new[]
            {
                new DjmainChartEvent
                {
                    Offset = 0,
                    Param0 = 0x00,
                    Param1 = 250
                },
                new DjmainChartEvent
                {
                    Offset = 0,
                    Param0 = 0x00,
                    Param1 = 5
                },
                new DjmainChartEvent
                {
                    Offset = 0,
                    Param0 = 0x10,
                    Param1 = 250
                },
                new DjmainChartEvent
                {
                    Offset = 0,
                    Param0 = 0x10,
                    Param1 = 5
                },
                new DjmainChartEvent
                {
                    Offset = offset
                }
            };

            // Act.
            var output = Subject.Decode(data, DjmainChartType.Beatmania);

            // Assert.
            output.Events.Should().HaveCount(1).And
                .Subject.First()[NumericData.LinearOffset].Should().Be(new BigRational(offset, 58));
        }

        [Test]
        public void Decode_SkipsSinglePlayNoteCount()
        {
            // Arrange.
            var offset = Create<ushort>();
            var data = new[]
            {
                new DjmainChartEvent
                {
                    Offset = 0,
                    Param0 = 0x00,
                    Param1 = 250
                },
                new DjmainChartEvent
                {
                    Offset = 0,
                    Param0 = 0x00,
                    Param1 = 5
                },
                new DjmainChartEvent
                {
                    Offset = offset
                }
            };

            // Act.
            var output = Subject.Decode(data, Create<DjmainChartType>());

            // Assert.
            output.Events.Should().HaveCount(1).And
                .Subject.First()[NumericData.LinearOffset].Should().Be(new BigRational(offset, 58));
        }

        [Test]
        public void Decode_UsesCorrectTiming()
        {
            // Arrange.
            const ushort timingValue = 1234;
            var data = new[]
            {
                new DjmainChartEvent
                {
                    Offset = timingValue
                }
            };

            // Act.
            var output = Subject.Decode(data, Create<DjmainChartType>());

            // Assert.
            output.Events.Should().HaveCount(1).And
                .Subject.First()[NumericData.LinearOffset].Should().Be(new BigRational(timingValue, 58));
        }
    }
}