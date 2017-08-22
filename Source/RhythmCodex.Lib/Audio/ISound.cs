using System.Collections.Generic;
using RhythmCodex.Charting;

namespace RhythmCodex.Audio
{
    public interface ISound : IMetadata
    {
        IList<ISample> Samples { get; set; }
    }
}