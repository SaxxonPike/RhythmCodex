﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shouldly;
using NUnit.Framework;
using RhythmCodex.Charting.Models;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Ssq.Streamers;
using RhythmCodex.Statistics;

namespace RhythmCodex.Ssq.Integration;

[TestFixture]
public class SsqDecodeIntegrationTests : BaseIntegrationFixture<SsqDecoder>
{
    private IEnumerable<Chart> DecodeCharts(byte[] data)
    {
        var ssqStreamer = Resolve<SsqStreamReader>();

        using var mem = new MemoryStream(data);
        return Subject.Decode(ssqStreamer.Read(mem)).ToArray();
    }

    [Test]
    public void DecodeOffsetSsq()
    {
        // Arrange.
        var data = GetArchiveResource("Ssq.offset.zip")
            .First()
            .Value;

        // Act.
        var charts = DecodeCharts(data);

        // Assert.
    }

    [Test]
    public void DecodeFreezeSsq()
    {
        // Arrange.
        var eventCounter = Resolve<EventCounter>();
        var expectedCombos = new[] {264, 373, 555, 85, 263, 347, 485};
        var expectedFreezes = new[] {2, 35, 2, 0, 8, 5, 2};
        var expectedShocks = new[] {0, 0, 0, 0, 0, 0, 0};
        var data = GetArchiveResource("Ssq.freeze.zip")
            .First()
            .Value;

        // Act.
        var charts = DecodeCharts(data);

        // Assert.
        charts.Select(c => eventCounter.CountCombos(c.Events)).ShouldBe(expectedCombos);
        charts.Select(c => eventCounter.CountComboFreezes(c.Events)).ShouldBe(expectedFreezes);
        charts.Select(c => eventCounter.CountComboShocks(c.Events)).ShouldBe(expectedShocks);
    }

    [Test]
    public void DecodeShockSsq()
    {
        // Arrange.
        var eventCounter = Resolve<EventCounter>();
        var expectedCombos = new[] {99, 180, 258, 368, 343, 207, 256, 336, 323};
        var expectedFreezes = new[] {4, 8, 3, 0, 0, 2, 7, 1, 1};
        var expectedShocks = new[] {0, 0, 0, 0, 37, 0, 0, 0, 29};
        var data = GetArchiveResource("Ssq.shock.zip")
            .First()
            .Value;

        // Act.
        var charts = DecodeCharts(data);

        // Assert.
        charts.Select(c => eventCounter.CountCombos(c.Events)).ShouldBe(expectedCombos);
        charts.Select(c => eventCounter.CountComboFreezes(c.Events)).ShouldBe(expectedFreezes);
        charts.Select(c => eventCounter.CountComboShocks(c.Events)).ShouldBe(expectedShocks);
    }
}