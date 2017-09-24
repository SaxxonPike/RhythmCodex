using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Attributes;
using RhythmCodex.Infrastructure;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    [TestFixture]
    public class NoteDecoderTests : BaseUnitTestFixture<NoteDecoder, INoteDecoder>
    {
        [Test]
        [TestCase(0, 2, 0, 0)]
        [TestCase(1, 2, 1, 0)]
        [TestCase(0, 2, 0, 1)]
        [TestCase(1, 2, 1, 1)]
        public void Decode_DecodesSingleEvent(int sourceColumn, int columnCount, int expectedColumn, int expectedPlayer)
        {
            // Arrange.
            var data = new[]
            {
                new Note { Column = sourceColumn, MetricOffset = BigRational.Zero, Type = '1'}
            };

            // Act.
            var output = Subject.Decode(data, columnCount).Single();

            // Assert.
            output[NumericData.SourceColumn].ShouldBeEquivalentTo(new BigRational((double)sourceColumn));
            output[NumericData.Player].ShouldBeEquivalentTo(new BigRational((double)expectedPlayer));
            output[NumericData.Column].ShouldBeEquivalentTo(new BigRational((double)expectedColumn));
        }
    }
}
