using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Sounds.Models;

public sealed class SoundBuilder : Metadata, IDisposable
{
    private bool _disposed;
    private readonly SampleBuilder[] _sampleBuilders;

    public static SoundBuilder FromSound(Sound sound, int? channelCount = null)
    {
        var resultChannels = channelCount ?? sound.Samples.Count;
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

    private SoundBuilder(SampleBuilder[] builders)
    {
        _sampleBuilders = builders;
    }
    
    public SoundBuilder(int sampleCount)
    {
        _sampleBuilders = new SampleBuilder[sampleCount];
        for (var i = 0; i < _sampleBuilders.Length; i++)
            _sampleBuilders[i] = new SampleBuilder();
    }

    public SoundBuilder(int sampleCount, int sampleLength)
        : this(sampleCount)
    {
        foreach (var sample in _sampleBuilders)
            sample.SetLength(sampleLength);
    }

    public IReadOnlyList<SampleBuilder> Samples =>
        _sampleBuilders;

    public void SetSampleLength(int sampleLength)
    {
        foreach (var sampleBuilder in _sampleBuilders)
            sampleBuilder.SetLength(sampleLength);
    }

    public Sound ToSound()
    {
        var result = new Sound
        {
            Samples = _sampleBuilders.Select(b => b.ToSample()).ToList()
        };

        result.CloneMetadataFrom(this);
        return result;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        foreach (var t in _sampleBuilders)
            t.Dispose();

        _disposed = true;
    }
}