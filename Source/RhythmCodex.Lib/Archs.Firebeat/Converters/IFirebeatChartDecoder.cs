using System.Collections.Generic;
using RhythmCodex.Archs.Firebeat.Models;
using RhythmCodex.Charts.Models;

namespace RhythmCodex.Archs.Firebeat.Converters;

public interface IFirebeatChartDecoder
{
    Chart Decode(IEnumerable<FirebeatChartEvent> events);
}