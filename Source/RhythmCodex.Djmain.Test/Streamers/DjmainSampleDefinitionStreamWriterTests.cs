using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Streamers
{
    [TestFixture]
    public class DjmainSampleDefinitionStreamWriterTests : BaseUnitTestFixture<DjmainSampleInfoStreamWriter, IDjmainSampleDefinitionStreamWriter>
    {
        [Test]
        public void Write_WritesSampleDefinitions()
        {
            // Arrange.
            Mock<IDjmainConfiguration>().Setup(x => x.MaxSampleDefinitions).Returns(3);

            var input = new[]
            {
                new DjmainSampleInfo
                {
                    Channel = 0x12,
                    Frequency = 0x3456,
                    ReverbVolume = 0x78,
                    Volume = 0x90,
                    Panning = 0x12,
                    Offset = 0x345678,
                    SampleType = 0x90,
                    Flags = 0x12
                },
                new DjmainSampleInfo
                {
                    Channel = 0x34,
                    Frequency = 0x5678,
                    ReverbVolume = 0x90,
                    Volume = 0x12,
                    Panning = 0x34,
                    Offset = 0x567890,
                    SampleType = 0x12,
                    Flags = 0x34
                }
            };

            var pairs = input.Select((e, i) => new KeyValuePair<int, DjmainSampleInfo>(i, e));

            var expected = new byte[]
            {
                0x12, 0x56, 0x34, 0x78, 0x90, 0x12, 0x78, 0x56, 0x34, 0x90, 0x12,
                0x34, 0x78, 0x56, 0x90, 0x12, 0x34, 0x90, 0x78, 0x56, 0x12, 0x34
            };

            using (var mem = new MemoryStream())
            {
                // Act.
                Subject.Write(mem, pairs);

                // Assert.
                var output = mem.ToArray();
                output.ShouldAllBeEquivalentTo(expected);
            }
        }
    }
}
