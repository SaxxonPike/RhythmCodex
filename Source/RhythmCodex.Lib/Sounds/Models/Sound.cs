using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RhythmCodex.Infrastructure;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Sounds.Models;

/// <summary>
/// Represents a set of <see cref="Sample"/> along with metadata.
/// </summary>
[PublicAPI]
[Model]
public class Sound : Metadata
{
    /// <summary>
    /// Samples contained within the sound.
    /// </summary>
    public List<Sample> Samples { get; set; } = [];

    /// <summary>
    /// Creates a clone of this sound, allocating new copies of the sample data.
    /// </summary>
    public Sound Clone()
    {
        var clone = new Sound
        {
            Samples = Samples.Select(s => s.Clone()).ToList()
        };

        clone.CloneMetadataFrom(this);
        return clone;
    }

    /// <summary>
    /// Replaces the metadata and sample data with a copy of the data within another <see cref="Sound"/>.
    /// </summary>
    public void ReplaceWithCopy(Sound other)
    {
        Samples.Clear();
        CloneMetadataFrom(other);

        foreach (var sample in other.Samples)
            Samples.Add(sample.Clone());
    }

    /// <summary>
    /// Replaces the metadata and sample data with the data within another <see cref="Sound"/> without
    /// allocating a new copy of the sample data.
    /// </summary>
    public void ReplaceNoCopy(Sound other)
    {
        Samples.Clear();
        CloneMetadataFrom(other);

        foreach (var sample in other.Samples)
            Samples.Add(sample);
    }

    /// <summary>
    /// Clears the samples from the sound.
    /// </summary>
    public void ClearSamples() =>
        Samples.Clear();
}