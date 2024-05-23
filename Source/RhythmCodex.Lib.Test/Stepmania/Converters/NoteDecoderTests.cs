using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters;

[TestFixture]
public class NoteDecoderTests : BaseUnitTestFixture<NoteDecoder, INoteDecoder>
{
    [Test]
    [TestCase(0, 2, 0, 0)]
    [TestCase(1, 2, 1, 0)]
    [TestCase(2, 2, 0, 1)]
    [TestCase(3, 2, 1, 1)]
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
        output[NumericData.SourceColumn].Should().BeEquivalentTo(new BigRational(sourceColumn, 1));
        output[NumericData.Player].Should().BeEquivalentTo(new BigRational(expectedPlayer, 1));
        output[NumericData.Column].Should().BeEquivalentTo(new BigRational(expectedColumn, 1));
    }
}