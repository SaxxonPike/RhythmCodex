using System.Collections.Generic;

namespace RhythmCodex.Charting
{
    public interface IChart : IMetadata
    {
        IList<IEvent> Events { get; }
    }
}