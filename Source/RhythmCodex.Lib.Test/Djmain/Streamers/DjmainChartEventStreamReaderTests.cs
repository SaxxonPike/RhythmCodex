using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Streamers
{
    [TestFixture]
    public class
        DjmainChartEventStreamReaderTests : BaseUnitTestFixture<DjmainChartEventStreamReader,
            IDjmainChartEventStreamReader>
    {
        [Test]
        public void Read_ReadsUntilEndMarker()
        {
            // Arrange.
            var data = new byte[]
            {
                0x00, 0x00, 0x00, 0x10,
                0x00, 0x00, 0x12, 0x34,
                0x12, 0x34, 0x56, 0x78,
                0x56, 0x78, 0x90, 0x12,
                0xFF, 0x7F, 0x00, 0x00
            };

            var expected = new[]
            {
                new DjmainChartEvent {Offset = 0x0000, Param0 = 0x00, Param1 = 0x10},
                new DjmainChartEvent {Offset = 0x0000, Param0 = 0x12, Param1 = 0x34},
                new DjmainChartEvent {Offset = 0x3412, Param0 = 0x56, Param1 = 0x78},
                new DjmainChartEvent {Offset = 0x7856, Param0 = 0x90, Param1 = 0x12}
            };

            // Act.
            DjmainChartEvent[] events;
            using (var mem = new MemoryStream(data))
            {
                events = Subject.Read(mem).ToArray();
            }

            // Assert.
            events.Should().BeEquivalentTo(expected);
        }
    }
}