using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Archs.Firebeat.Models;

[Model]
public class FirebeatChart
{
    public int Id { get; set; }
    public int Offset { get; set; }
    public byte[] Header { get; set; } = [];
    public List<FirebeatChartEvent> Events { get; set; } = [];
}