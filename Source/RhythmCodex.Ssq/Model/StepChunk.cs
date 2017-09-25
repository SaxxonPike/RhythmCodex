using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ssq.Model
{
    [Model]
    public class StepChunk
    {
        public IList<Step> Steps { get; set; }
        public int Id { get; set; }
    }
}