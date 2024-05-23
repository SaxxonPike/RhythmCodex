using System.Collections.Generic;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Charting.Models;

public interface IChart : IMetadata
{
    IList<IEvent> Events { get; }
}