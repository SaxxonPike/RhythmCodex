using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Attributes;

namespace RhythmCodex.Stepmania.Converters
{
    [TestFixture]
    public class GrooveRadarEncoderTests : BaseUnitTestFixture<GrooveRadarEncoder, IGrooveRadarEncoder>
    {
        [Test]
        public void Encode_EncodesValuesInCorrectOrder()
        {
            // Arrange.
            var air = $"{Create<float>()}";
            var chaos = $"{Create<float>()}";
            var freeze = $"{Create<float>()}";
            var stream = $"{Create<float>()}";
            var voltage = $"{Create<float>()}";

            var data = new Metadata
            {
                [NotesCommandTag.AirTag] = air,
                [NotesCommandTag.ChaosTag] = chaos,
                [NotesCommandTag.FreezeTag] = freeze,
                [NotesCommandTag.StreamTag] = stream,
                [NotesCommandTag.VoltageTag] = voltage
            };

            // Act.
            var output = Subject.Encode(data);

            // Assert.
            output.ShouldBeEquivalentTo($"{stream},{voltage},{air},{freeze},{chaos}");
        }
    }
}
