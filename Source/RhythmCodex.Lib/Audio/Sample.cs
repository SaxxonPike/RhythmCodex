using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Audio
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