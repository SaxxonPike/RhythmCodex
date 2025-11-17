using System;
using JetBrains.Annotations;
using RhythmCodex.Infrastructure;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Sounds.Models;

/// <summary>
/// Represents a single channel of raw audio data along with metadata.
/// </summary>
[PublicAPI]
[Model]
public class Sample : Metadata
{
    /// <summary>
    /// Raw audio data.
    /// </summary>
    public ReadOnlyMemory<float> Data { get; set; }

    /// <summary>
    /// Allocates a copy of the sample and metadata.
    /// </summary>
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