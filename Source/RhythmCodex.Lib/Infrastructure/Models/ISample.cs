using System.Collections.Generic;
using RhythmCodex.Charting;

namespace RhythmCodex.Infrastructure.Models
{
    public interface ISample : IMetadata
    {
        IList<float> Data { get; set; }
        ISample Clone();
    }
}