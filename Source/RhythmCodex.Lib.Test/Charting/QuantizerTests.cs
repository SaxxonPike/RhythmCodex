using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Charting;

[TestFixture]
public class QuantizerTests : BaseUnitTestFixture<Quantizer, IQuantizer>
{
    /// <summary>
    ///     Parse a string fraction such as "1/2" into a BigRational.
    /// </summary>
    private static BigRational Parse(string value)
    {
        var input = value.Split('/').Select(s => Convert.ToInt32(s)).ToArray();
        return new BigRational(input[0], input[1]);
    }

    [Test]
    [TestCase("6/32", "12/32", "18/32", 16)]
    [TestCase("1/2", "1/4", "1/8", 8)]
    [TestCase("1/2", "1/3", "1/4", 12)]
    [TestCase("2/8", "4/8", "6/8", 4)]
    [TestCase("1/7", "2/9", "7/13", 819)]
    public void GetQuantization_Quantizes_WithDefaultSettings(string one, string two, string three, int expected)
    {
        var data = new[]
        {
            Parse(one),
            Parse(two),
            Parse(three)
        };

        // Act.
        var result = Subject.GetQuantization(data);

        // Assert.
        result.Should().Be(expected);
    }
}