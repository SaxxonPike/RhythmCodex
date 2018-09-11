using System.Collections.Generic;
using RhythmCodex.Attributes;
using RhythmCodex.Extensions;

namespace RhythmCodex.Infrastructure.Models
{
    [Model]
    public class Sample : Metadata, ISample
    {
        public IList<float> Data { get; set; }
        
        public ISample Clone()
        {
            var clone = new Sample
            {
                Data = (float[])Data.AsArray().Clone()
            };
            
            clone.CloneMetadataFrom(this);
            return clone;
        }
    }
}