using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ssq.Model;

[Model]
public class StepChunk
{
    public List<Step> Steps { get; set; }
    public int Id { get; set; }
}