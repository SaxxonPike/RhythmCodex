using System.Collections.Generic;
using RhythmCodex.Charting;

namespace RhythmCodex.Infrastructure.Models
{
    public interface ISound : IMetadata
    {
        IList<ISample> Samples { get; set; }
    }
}