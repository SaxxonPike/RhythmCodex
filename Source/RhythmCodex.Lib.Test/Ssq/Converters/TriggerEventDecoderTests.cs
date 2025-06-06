﻿using System.Linq;
using Shouldly;
using NUnit.Framework;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[TestFixture]
public class TriggerEventDecoderTests : BaseUnitTestFixture<TriggerEventDecoder, ITriggerEventDecoder>
{
    [Test]
    public void Decode_ConvertsTriggersCorrectly()
    {
        // Arrange.
        var triggers = new[]
        {
            new Trigger {Id = 0x2345, MetricOffset = 0x01020304},
            new Trigger {Id = 0x3456, MetricOffset = 0x02030405},
            new Trigger {Id = 0x4567, MetricOffset = 0x03040506}
        };

        var expected = new[]
        {
            new Event
            {
                [NumericData.Trigger] = 0x2345,
                [NumericData.MetricOffset] = (BigRational) 0x01020304 / 4096
            },
            new Event
            {
                [NumericData.Trigger] = 0x3456,
                [NumericData.MetricOffset] = (BigRational) 0x02030405 / 4096
            },
            new Event {[NumericData.Trigger] = 0x4567, [NumericData.MetricOffset] = (BigRational) 0x03040506 / 4096}
        };

        // Act.
        var result = Subject.Decode(triggers).ToArray();

        // Assert.
        result.Length.ShouldBe(expected.Length);
        var resultMatches = Enumerable.Range(0, expected.Length)
            .Select(i => result[i].MetadataEquals(expected[i]));
        resultMatches.ShouldBe(Enumerable.Repeat(true, expected.Length));
    }
}