using System;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace RhythmCodex.Xa.Converters
{
    [TestFixture]
    public class XaFrameSplitterTests : BaseUnitTestFixture<XaFrameSplitter, IXaFrameSplitter>
    {
        private static readonly byte[] Frame1 =
        {
            0x27, 0x27, 0x27, 0x27, 0x27, 0x27, 0x27, 0x27,
            0x26, 0x27, 0x27, 0x27, 0x26, 0x27, 0x27, 0x27,
            0x0F, 0x34, 0x21, 0x31, 0xED, 0xF1, 0x00, 0x46,
            0x0F, 0xBC, 0xE0, 0x35, 0x02, 0xCA, 0xF0, 0x00,
            0x05, 0x2E, 0x2F, 0xDF, 0x24, 0x41, 0x21, 0xE0,
            0xCE, 0x12, 0x20, 0xD2, 0xB9, 0xFF, 0x00, 0x11,
            0xEE, 0xCF, 0x11, 0x31, 0x03, 0xEF, 0x33, 0x31,
            0x02, 0x01, 0x11, 0x21, 0xFF, 0x22, 0xE0, 0xDD,
            0x10, 0x32, 0xDE, 0xAA, 0x01, 0x11, 0x1F, 0xCD,
            0x01, 0xF2, 0x30, 0x11, 0xFF, 0xB1, 0x21, 0x42,
            0xDF, 0xAE, 0x11, 0x33, 0x31, 0xB9, 0xEE, 0x00,
            0x44, 0x0F, 0xBE, 0xEF, 0x32, 0x55, 0xAF, 0x0D,
            0x1F, 0x21, 0x10, 0xFF, 0xFC, 0xCC, 0x30, 0x0C,
            0xEE, 0xCE, 0x40, 0x0B, 0x01, 0x16, 0x44, 0x1F,
            0x2F, 0x35, 0x11, 0x01, 0x10, 0x1F, 0xBE, 0xE1,
            0x2F, 0xEB, 0xAD, 0xFF, 0x30, 0x1E, 0xDE, 0x1F
        };

        private static readonly object[] Frame1Data =
        {
            new object[]
            {
                Frame1,
                0,
                0x27,
                new byte[]
                {
                    0xF, 0xD, 0xF, 0x2, 
                    0x5, 0x4, 0xE, 0x9, 
                    0xE, 0x3, 0x2, 0xF, 
                    0x0, 0x1, 0x1, 0xF, 
                    0xF, 0x1, 0x4, 0x2,
                    0xF, 0xC, 0xE, 0x1, 
                    0xF, 0x0, 0xF, 0x0
                }
            },
            new object[]
            {
                Frame1,
                1,
                0x27,
                new byte[]
                {
                    0x0, 0xE, 0x0, 0x0, 
                    0x0, 0x2, 0xC, 0xB, 
                    0xE, 0x0, 0x0, 0xF, 
                    0x1, 0x0, 0x0, 0xF, 
                    0xD, 0x3, 0x4, 0x3,
                    0x1, 0xF, 0xE, 0x0, 
                    0x2, 0x1, 0x2, 0x3
                }
            },
            new object[]
            {
                Frame1,
                2,
                0x27,
                new byte[]
                {
                    0x4, 0x1, 0xC, 0xA, 
                    0xE, 0x1, 0x2, 0xF, 
                    0xF, 0xF, 0x1, 0x2, 
                    0x2, 0x1, 0x2, 0x1, 
                    0xE, 0x9, 0xF, 0x5,
                    0x1, 0xC, 0xE, 0x6, 
                    0x5, 0xF, 0xB, 0xE
                }
            },
        };

        [Test]
        [TestCaseSource(nameof(Frame1Data))]
        public void Test1(byte[] frame, int channel, int expectedStatus, byte[] expectedData)
        {
            Subject.GetStatus(frame, channel).Should().Be(expectedStatus);
            Subject.GetData(frame, channel).Should().BeEquivalentTo(expectedData);
        }
    }
}