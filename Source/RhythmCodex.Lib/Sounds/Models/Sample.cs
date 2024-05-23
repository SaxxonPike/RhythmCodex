using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Sounds.Models;

[Model]
public class Sample : Metadata
{
    public Memory<float> Data { get; set; }
        
    public Sample Clone()
    {
        var clone = new Sample
        {
            Data = Data.ToArray()
        };
            
        clone.CloneMetadataFrom(this);
        return clone;
    }
}