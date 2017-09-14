using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace RhythmCodex.Djmain.Streamers
{
    [TestFixture]
    public class AudioStreamWriterTests : BaseUnitTestFixture<AudioStreamWriter, IAudioStreamWriter>
    {
        [Test]
        public void WriteDpcm_WritesAllDataPlusEndMarker()
        {
            // Arrange.
            var data = new byte[]
            {
                0x12, 0x34, 0x56, 0x78
            };

            var expected = new byte[]
            {
                0x12, 0x34, 0x56, 0x78,
                0x88, 0x88, 0x88, 0x88
            };

            // Act.
            byte[] output;
            using (var mem = new MemoryStream())
            {
                Subject.WriteDpcm(mem, data);
                mem.Flush();
                output = mem.ToArray();
            }

            // Assert.
            output.ShouldAllBeEquivalentTo(expected);
        }

        [Test]
        public void WritePcm8_WritesAllDataPlusEndMarker()
        {
            // Arrange.
            var data = new byte[]
            {
                0x12, 0x34, 0x56, 0x78
            };

            var expected = new byte[]
            {
                0x12, 0x34, 0x56, 0x78,
                0x80, 0x80, 0x80, 0x80,
                0x80, 0x80, 0x80, 0x80
            };

            // Act.
            byte[] output;
            using (var mem = new MemoryStream())
            {
                Subject.WritePcm8(mem, data);
                mem.Flush();
                output = mem.ToArray();
            }

            // Assert.
            output.ShouldAllBeEquivalentTo(expected);
        }

        [Test]
        public void WritePcm16_WritesAllDataPlusEndMarker()
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
                Subject.WritePcm16(mem, data);
                mem.Flush();
                output = mem.ToArray();
            }

            // Assert.
            output.ShouldAllBeEquivalentTo(expected);
        }

        [Test]
        public void WritePcm16_WritesMisalignedDataPlusEndMarker()
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
                Subject.WritePcm16(mem, data);
                mem.Flush();
                output = mem.ToArray();
            }

            // Assert.
            output.ShouldAllBeEquivalentTo(expected);
        }

    }
}
