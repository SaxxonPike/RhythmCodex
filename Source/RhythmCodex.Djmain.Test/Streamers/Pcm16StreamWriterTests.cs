using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace RhythmCodex.Djmain.Streamers
{
    [TestFixture]
    public class Pcm16StreamWriterTests : BaseUnitTestFixture<Pcm16StreamWriter>
    {
        [Test]
        public void Write_WritesAllDataPlusEndMarker()
        {
            // Arrange.
            var data = new byte[]
            {
                0x12, 0x34, 0x56, 0x78
            };

            var expected = new byte[]
            {
                0x12, 0x34, 0x56, 0x78,
                0x00, 0x80, 0x00, 0x80,
                0x00, 0x80, 0x00, 0x80,
                0x00, 0x80, 0x00, 0x80,
                0x00, 0x80, 0x00, 0x80
            };

            // Act.
            byte[] output;
            using (var mem = new MemoryStream())
            {
                Subject.Write(mem, data);
                mem.Flush();
                output = mem.ToArray();
            }

            // Assert.
            output.ShouldAllBeEquivalentTo(expected);
        }

        [Test]
        public void Write_WritesMisalignedDataPlusEndMarker()
        {
            // Arrange.
            var data = new byte[]
            {
                0x12, 0x34, 0x56
            };

            var expected = new byte[]
            {
                0x12, 0x34,
                0x00, 0x80, 0x00, 0x80,
                0x00, 0x80, 0x00, 0x80,
                0x00, 0x80, 0x00, 0x80,
                0x00, 0x80, 0x00, 0x80
            };

            // Act.
            byte[] output;
            using (var mem = new MemoryStream())
            {
                Subject.Write(mem, data);
                mem.Flush();
                output = mem.ToArray();
            }

            // Assert.
            output.ShouldAllBeEquivalentTo(expected);
        }
    }
}
