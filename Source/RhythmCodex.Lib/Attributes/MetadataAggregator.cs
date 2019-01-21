using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Attributes
{
    [Service]
    public class MetadataAggregator : IMetadataAggregator
    {
        public IMetadata Aggregate(IEnumerable<IMetadata> metadatas)
        {
            var result = new Metadata();
            foreach (var metadata in metadatas)
                metadata.CopyTo(result);
            return result;
        }
    }
}