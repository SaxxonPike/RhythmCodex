using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Sounds.Models
{
    public interface ISound : IMetadata
    {
        IList<ISample> Samples { get; set; }
    }
}