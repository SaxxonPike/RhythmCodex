using System.Collections.Generic;

namespace RhythmCodex.Meta.Models
{
    public interface IMetadataAggregator
    {
        IMetadata Aggregate(IEnumerable<IMetadata> metadatas);
    }
}