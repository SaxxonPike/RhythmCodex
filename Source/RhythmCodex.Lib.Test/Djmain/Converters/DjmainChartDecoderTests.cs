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
        [TestCase(0x05, 0)]
        [TestCase(0x15, 0)]
        [TestCase(0x25, 1)]
        [TestCase(0x35, 2)]
        [TestCase(0x45, 3)]
        [TestCase(0x55, 4)]
        [TestCase(0x65, 5)]
        [TestCase(0x75, 6)]
        [TestCase(0x85, 7)]
        [TestCase(0x95, 8)]
        [TestCase(0xA5, 9)]
        [TestCase(0xB5, 10)]
        [TestCase(0xC5, 11)]
        [TestCase(0xD5, 12)]
        [TestCase(0xE5, 13)]
        [TestCase(0xF5, 14)]
        public void Decode_DecodesBgm(byte param0, int expectedPanning)
        {
            // Arrange.
            var offset = Create<ushort>();
            var param1 = Create<byte>();
            var data = new[]
            {
                new DjmainChartEvent
                {
                    Offset = offset,
                    Param0 = param0,
                    Param1 = param1
                }
            };

            // Act.
            var output = Subject.Decode(data);

            // Assert.
            output.Events.Should().HaveCount(1);
            var ev = output.Events.Single();
            ev[NumericData.PlaySound].Value.Should().Be(param1 - 1);
            ev[NumericData.Panning].Value.Should().Be(new BigRational(expectedPanning, 14));
        }

        [Test]
        public void Decode_DecodesEnd()
        {
            // Arrange.
            var offset = Create<ushort>();
            const int param0 = 0x04;
            var param1 = Create<byte>();
            var data = new[]
            {
                new DjmainChartEvent
                {
                    Offset = offset,
                    Param0 = param0,
                    Param1 = param1
                }
            };

            // Act.
            var output = Subject.Decode(data);

            // Assert.
            output.Events.Should().HaveCount(1);
            var ev = output.Events.Single();
            ev[FlagData.End].Value.Should().BeTrue();
        }

        [Test]
        [TestCase(0xE0, 0)]
        [TestCase(0xF0, 1)]
        public void Decode_DecodesFreeScratch(byte param0, int expectedPlayer)
        {
            // Arrange.
            var offset = Create<ushort>();
            var param1 = Create<byte>();
            var data = new[]
            {
                new DjmainChartEvent
                {
                    Offset = offset,
                    Param0 = param0,
                    Param1 = param1
                }
            };

            // Act.
            var output = Subject.Decode(data);

            // Assert.
            output.Events.Should().HaveCount(1);
            var ev = output.Events.Single();
            ev[FlagData.FreeZone].Should().Be(true);
            ev[NumericData.Player].Value.Should().Be(expectedPlayer);
        }

        [Test]
        [TestCase(0xC0, 0)]
        [TestCase(0xD0, 1)]
        public void Decode_DecodesMeasure(byte param0, int expectedPlayer)
        {
            // Arrange.
            var offset = Create<ushort>();
            var param1 = Create<byte>();
            var data = new[]
            {
                new DjmainChartEvent
                {
                    Offset = offset,
                    Param0 = param0,
                    Param1 = param1
                }
            };

            // Act.
            var output = Subject.Decode(data);

            // Assert.
            output.Events.Should().HaveCount(1);
            var ev = output.Events.Single();
            ev[FlagData.Measure].Should().Be(true);
            ev[NumericData.Player].Value.Should().Be(expectedPlayer);
        }

        [Test]
        [TestCase(0x00, 0, 0)]
        [TestCase(0x10, 1, 0)]
        [TestCase(0x20, 0, 1)]
        [TestCase(0x30, 1, 1)]
        [TestCase(0x40, 0, 2)]
        [TestCase(0x50, 1, 2)]
        [TestCase(0x60, 0, 3)]
        [TestCase(0x70, 1, 3)]
        [TestCase(0x80, 0, 4)]
        [TestCase(0x90, 1, 4)]
        public void Decode_DecodesNote(byte param0, int expectedPlayer, int expectedColumn)
        {
            // Arrange.
            var offset = Create<ushort>();
            var param1 = Create<byte>();
            var data = new[]
            {
                new DjmainChartEvent
                {
                    Offset = offset,
                    Param0 = param0,
                    Param1 = param1
                }
            };

            // Act.
            var output = Subject.Decode(data);

            // Assert.
            output.Events.Should().HaveCount(1);
            var ev = output.Events.Single();
            ev[FlagData.Note].Should().Be(true);
            ev[NumericData.Player].Value.Should().Be(expectedPlayer);
            ev[NumericData.Column].Value.Should().Be(expectedColumn);
        }

        [Test]
        [TestCase(0x01, 0, 0)]
        [TestCase(0x11, 1, 0)]
        [TestCase(0x21, 0, 1)]
        [TestCase(0x31, 1, 1)]
        [TestCase(0x41, 0, 2)]
        [TestCase(0x51, 1, 2)]
        [TestCase(0x61, 0, 3)]
        [TestCase(0x71, 1, 3)]
        [TestCase(0x81, 0, 4)]
        [TestCase(0x91, 1, 4)]
        public void Decode_DecodesNoteSoundSelect(byte param0, int expectedPlayer, int expectedColumn)
        {
            // Arrange.
            var offset = Create<ushort>();
            var param1 = Create<byte>();
            var data = new[]
            {
                new DjmainChartEvent
                {
                    Offset = offset,
                    Param0 = param0,
                    Param1 = param1
                }
            };

            // Act.
            var output = Subject.Decode(data);

            // Assert.
            output.Events.Should().HaveCount(1);
            var ev = output.Events.Single();
            ev[NumericData.LoadSound].Value.Should().Be(param1 - 1);
            ev[NumericData.Player].Value.Should().Be(expectedPlayer);
            ev[NumericData.Column].Value.Should().Be(expectedColumn);
        }

        [Test]
        [TestCase(0xA0, 0)]
        [TestCase(0xB0, 1)]
        public void Decode_DecodesScratch(byte param0, int expectedPlayer)
        {
            // Arrange.
            var offset = Create<ushort>();
            var param1 = Create<byte>();
            var data = new[]
            {
                new DjmainChartEvent
                {
                    Offset = offset,
                    Param0 = param0,
                    Param1 = param1
                }
            };

            // Act.
            var output = Subject.Decode(data);

            // Assert.
            output.Events.Should().HaveCount(1);
            var ev = output.Events.Single();
            ev[FlagData.Scratch].Should().Be(true);
            ev[NumericData.Player].Value.Should().Be(expectedPlayer);
        }

        [Test]
        [TestCase(0xA1, 0)]
        [TestCase(0xB1, 1)]
        [TestCase(0xE1, 0)]
        [TestCase(0xF1, 1)]
        public void Decode_DecodesScratchSoundSelect(byte param0, int expectedPlayer)
        {
            // Arrange.
            var offset = Create<ushort>();
            var param1 = Create<byte>();
            var data = new[]
            {
                new DjmainChartEvent
                {
                    Offset = offset,
                    Param0 = param0,
                    Param1 = param1
                }
            };

            // Act.
            var output = Subject.Decode(data);

            // Assert.
            output.Events.Should().HaveCount(1);
            var ev = output.Events.Single();
            ev[FlagData.Scratch].Value.Should().BeTrue();
            ev[NumericData.LoadSound].Value.Should().Be(param1 - 1);
            ev[NumericData.Player].Value.Should().Be(expectedPlayer);
        }

        [Test]
        [TestCase(0x02, 0x01, 0x001)]
        [TestCase(0x02, 0x04, 0x004)]
        [TestCase(0x02, 0x11, 0x011)]
        [TestCase(0x02, 0x44, 0x044)]
        [TestCase(0x12, 0x01, 0x101)]
        [TestCase(0x12, 0x04, 0x104)]
        [TestCase(0x12, 0x11, 0x111)]
        [TestCase(0x12, 0x44, 0x144)]
        [TestCase(0x42, 0x01, 0x401)]
        [TestCase(0x42, 0x04, 0x404)]
        [TestCase(0x42, 0x11, 0x411)]
        [TestCase(0x42, 0x44, 0x444)]
        public void Decode_DecodesTempo(byte param0, byte param1, int expectedTempo)
        {
            // Arrange.
            var offset = Create<ushort>();
            var data = new[]
            {
                new DjmainChartEvent
                {
                    Offset = offset,
                    Param0 = param0,
                    Param1 = param1
                }
            };

            // Act.
            var output = Subject.Decode(data);

            // Assert.
            output.Events.Should().HaveCount(1);
            var ev = output.Events.Single();
            ev[NumericData.Bpm].Value.Should().Be(expectedTempo);
        }

        [Test]
        public void Decode_SkipsDoublePlayNoteCount()
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
            var output = Subject.Decode(data);

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
            var output = Subject.Decode(data);

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
            var output = Subject.Decode(data);

            // Assert.
            output.Events.Should().HaveCount(1).And
                .Subject.First()[NumericData.LinearOffset].Should().Be(new BigRational(timingValue, 58));
        }
    }
}