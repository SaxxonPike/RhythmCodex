using System.Linq;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
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
                new Chunk { Data = Fixture.Create<byte[]>(), Parameter0 = Parameter0.Timings },
                new Chunk { Data = Fixture.Create<byte[]>(), Parameter0 = Parameter0.Steps },
            };
            var timings = Fixture.CreateMany<Timing>().ToList();
            
            Mock<ITimingChunkDecoder>(mock =>
            {
                mock.Setup(m => m.Convert(It.IsAny<byte[]>())).Returns(timings);
            });

            // Act.
            Subject.Decode(chunks);
            
            // Assert.
            
            // [TODO]
        }
    }
}
