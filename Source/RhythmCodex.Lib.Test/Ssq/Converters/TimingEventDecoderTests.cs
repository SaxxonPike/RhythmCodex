﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RhythmCodex.Charting.Models;
using RhythmCodex.Meta.Models;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[TestFixture]
public class TimingEventDecoderTests : BaseUnitTestFixture<TimingEventDecoder, ITimingEventDecoder>
{
    [Test]
    public void Decode_ConvertsTimingsCorrectly()
    {
        // Arrange.
        List<Timing> data =
        [
            new Timing { LinearOffset = 0, MetricOffset = 0 },
            new Timing { LinearOffset = 100, MetricOffset = 4096 },
            new Timing { LinearOffset = 150, MetricOffset = 8192 }
        ];

        var timings = new TimingChunk
        {
            Timings = data,
            Rate = 100
        };

        List<Event> expected =
        [
            new Event { [NumericData.Bpm] = 240, [NumericData.LinearOffset] = 0, [NumericData.MetricOffset] = 0 },
            new Event { [NumericData.Bpm] = 480, [NumericData.LinearOffset] = 1, [NumericData.MetricOffset] = 1 }
        ];

        // Act.
        var result = Subject.Decode(timings).ToArray();

        // Assert.
        result.Should().HaveCount(expected.Count);
        var resultMatches = Enumerable.Range(0, expected.Count)
            .Select(i => result[i].MetadataEquals(expected[i]));
        resultMatches.Should().BeEquivalentTo(Enumerable.Repeat(true, expected.Count));
    }
}