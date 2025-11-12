using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Sounds.Models;

/// <summary>
/// Allows for building <see cref="Sound"/> objects using <see cref="SampleBuilder"/>.
/// </summary>
[PublicAPI]
public sealed class SoundBuilder : Metadata, IDisposable
{
    private bool _disposed;
    private readonly SampleBuilder[] _sampleBuilders;

    /// <summary>
    /// Creates a <see cref="SoundBuilder"/> using a copy of samples and metadata from a <see cref="Sound"/>.
    /// </summary>
    /// <param name="sound">
    /// Sound to copy.
    /// </param>
    /// <param name="sampleCount">
    /// Number of samples. Input samples will be processed round-robin if channel count does not match the input.
    /// This can be left null to automatically match the input sample count.
    /// </param>
    public static SoundBuilder FromSound(Sound sound, int? sampleCount = null)
    {
        var resultChannels = sampleCount ?? sound.Samples.Count;
        var builders = new SampleBuilder[resultChannels];

        var result = new SoundBuilder(builders);
        result.CloneMetadataFrom(sound);

        if (resultChannels > 0)
        {
            var c = 0;

            for (var i = 0; i < resultChannels; i++)
            {
                builders[i] = SampleBuilder.FromSample(sound.Samples[c++]);
                c %= sound.Samples.Count;
            }
        }

        return result;
    }

    /// <summary>
    /// Initialize a <see cref="SoundBuilder"/> with the specified <see cref="SampleBuilder"/> objects.
    /// </summary>
    private SoundBuilder(SampleBuilder[] builders)
    {
        _sampleBuilders = builders;
    }

    /// <summary>
    /// Creates a new <see cref="SoundBuilder"/>.
    /// </summary>
    /// <param name="sampleCount">
    /// Number of sample builders to initialize.
    /// </param>
    public SoundBuilder(int sampleCount)
    {
        _sampleBuilders = new SampleBuilder[sampleCount];
        for (var i = 0; i < _sampleBuilders.Length; i++)
            _sampleBuilders[i] = new SampleBuilder();
    }

    /// <summary>
    /// Creates a new <see cref="SoundBuilder"/>.
    /// </summary>
    /// <param name="sampleCount">
    /// Number of sample builders to initialize.
    /// </param>
    /// <param name="sampleLength">
    /// Initial length of the sample data. Setting this if it is known in advance can
    /// benefit performance.
    /// </param>
    public SoundBuilder(int sampleCount, int sampleLength)
        : this(sampleCount)
    {
        foreach (var sample in _sampleBuilders)
            sample.SetLength(sampleLength);
    }

    /// <summary>
    /// Gets the contained sample builders.
    /// </summary>
    public IReadOnlyList<SampleBuilder> Samples
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            return _sampleBuilders;
        }
    }

    /// <summary>
    /// Initializes the data length for each sample builder.
    /// </summary>
    /// <param name="sampleLength">
    /// Number of values.
    /// </param>
    public void SetSampleLength(int sampleLength)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        foreach (var sampleBuilder in _sampleBuilders)
            sampleBuilder.SetLength(sampleLength);
    }

    /// <summary>
    /// Copies data and metadata into a new <see cref="Sound"/>.
    /// </summary>
    public Sound ToSound()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var result = new Sound
        {
            Samples = _sampleBuilders.Select(b => b.ToSample()).ToList()
        };

        result.CloneMetadataFrom(this);
        return result;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        if (_disposed)
            return;

        foreach (var t in _sampleBuilders)
            t.Dispose();

        _sampleBuilders.AsSpan().Clear();
        _disposed = true;
    }
}