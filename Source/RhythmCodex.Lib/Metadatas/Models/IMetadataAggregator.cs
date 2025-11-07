using System.Collections.Generic;

namespace RhythmCodex.Metadatas.Models;

public interface IMetadataAggregator
{
    IMetadata Aggregate(IEnumerable<IMetadata> metadatas);
}