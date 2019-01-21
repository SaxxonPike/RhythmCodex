using System.Collections.Generic;
using RhythmCodex.Charting;

namespace RhythmCodex.Attributes
{
    public interface IMetadataAggregator
    {
        IMetadata Aggregate(IEnumerable<IMetadata> metadatas);
    }
}