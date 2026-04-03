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

    /// <summary>
    /// Creates a clone of the sample that uses the specified data. The data is not copied.  
    /// </summary>
    public Sample CloneWithData(ReadOnlyMemory<float> data)
    {
        var clone = new Sample
        {
            Data = data
        };

        clone.CloneMetadataFrom(this);
        return clone;
    }

    /// <summary>
    /// Generates a hash code of source volume/panning values.
    /// </summary>
    public int CalculateSourceVolumePanHash()
    {
        var hc = new HashCode();

        if (this[NumericData.SourceVolume] is { } sourceVol)
            hc.Add(sourceVol);
        else
            hc.Add(BigRational.One);

        if (this[NumericData.SourcePanning] is { } sourcePan)
            hc.Add(sourcePan);
        else
            hc.Add(BigRational.OneHalf);

        return hc.ToHashCode();
    }
}