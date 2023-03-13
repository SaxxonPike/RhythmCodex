using System.Collections.Generic;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Sounds.Models;

public interface ISample : IMetadata
{
    IList<float> Data { get; set; }
    ISample Clone();
}