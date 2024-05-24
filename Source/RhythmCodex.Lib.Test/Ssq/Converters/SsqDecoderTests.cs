using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[TestFixture]
public class SsqDecoderTests : BaseUnitTestFixture<SsqDecoder, ISsqDecoder>
{
    [Test]
    public void Decode_DoesStuff()
    {
        // Arrange.
        var chunks = new[]
        {
            new SsqChunk {Data = Create<byte[]>(), Parameter0 = Parameter0.Timings},
            new SsqChunk {Data = Create<byte[]>(), Parameter0 = Parameter0.Steps}
        };

        var timings = CreateMany<Timing>().ToList();
        var steps = CreateMany<Step>().ToList();

        Mock<ITimingChunkDecoder>(mock =>
        {
            mock.Setup(m => m.Convert(It.IsAny<byte[]>()))
                .Returns(timings);
        });

        Mock<IStepChunkDecoder>(mock =>
        {
            mock.Setup(m => m.Convert(It.IsAny<byte[]>()))
                .Returns(steps);
        });

        Mock<ISsqChunkFilter>(mock =>
        {
            mock.Setup(x => x.GetTimings(It.IsAny<IEnumerable<SsqChunk>>()))
                .Returns(new TimingChunk());
            mock.Setup(x => x.GetSteps(It.IsAny<IEnumerable<SsqChunk>>()))
                .Returns(Enumerable.Empty<StepChunk>().ToList());
            mock.Setup(x => x.GetInfos(It.IsAny<IEnumerable<SsqChunk>>()))
                .Returns(Enumerable.Empty<SsqInfoChunk>().ToList());
        });

        // Act.
        var result = Subject.Decode(chunks).ToList();

        // Assert.

        // [TODO]
    }
}