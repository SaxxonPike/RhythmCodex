using System.Linq;
using Moq;
using NUnit.Framework;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [TestFixture]
    public class SsqDecoderTests : BaseUnitTestFixture<SsqDecoder>
    {
        [Test]
        public void Decode_DoesStuff()
        {
            // Arrange.
            var chunks = new Chunk?[]
            {
                new Chunk { Data = Create<byte[]>(), Parameter0 = Parameter0.Timings },
                new Chunk { Data = Create<byte[]>(), Parameter0 = Parameter0.Steps },
            };
            
            var timings = CreateMany<Timing>().ToList();
            var steps = CreateMany<Step>().ToList();
            
            Mock<ITimingChunkDecoder>(mock =>
            {
                mock.Setup(m => m.Convert(It.IsAny<byte[]>())).Returns(timings);
            });

            Mock<IStepChunkDecoder>(mock =>
            {
                mock.Setup(m => m.Convert(It.IsAny<byte[]>())).Returns(steps);
            });

            // Act.
            var result = Subject.Decode(chunks).ToList();
            
            // Assert.
            
            // [TODO]
        }
    }
}
