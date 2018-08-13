using System.Collections.Generic;
using RhythmCodex.Charting;

namespace RhythmCodex.Audio
{
    public interface ISample : IMetadata
    {
        IList<float> Data { get; set; }
        ISample Clone();
    }
}