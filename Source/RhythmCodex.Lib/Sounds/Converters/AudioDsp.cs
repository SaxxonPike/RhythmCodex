using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Providers;

namespace RhythmCodex.Sounds.Converters;

[Service]
public class AudioDsp : IAudioDsp
{
    public Sound? ApplyPanVolume(Sound sound, BigRational volume, BigRational panning)
    {
        var newSound = new Sound
        {
            Samples = [..sound.Samples]
        };

        newSound.CloneMetadataFrom(sound);
        newSound[NumericData.Volume] = volume;
        newSound[NumericData.Panning] = panning;

        ApplyEffectsInternal(newSound);
        return newSound;
    }

    public Sound? ApplyResampling(Sound? sound, IResampler resampler, BigRational rate)
    {
        if (sound == null || !sound.Samples.Any())
            return null;

        if (rate <= BigRational.Zero ||
            sound[NumericData.Rate] == rate ||
            sound[NumericData.Rate] == 0 ||
            sound.Samples == null ||
            !sound.Samples.Any() ||
            sound.Samples.Any(sa => sa[NumericData.Rate] != null && sa[NumericData.Rate] <= 0))
            return sound;

        var targetRate = (float)(double)rate;
        var samples = new List<Sample>(sound.Samples);
        var result = new Sound
        {
            Samples = samples.Select(s =>
            {
                var sourceRate = (float)(double)(s[NumericData.Rate] ?? sound[NumericData.Rate]);
                var sample = new Sample
                {
                    Data = resampler.Resample(s.Data.Span, sourceRate, targetRate)
                };
                sample.CloneMetadataFrom(s);
                sample[NumericData.Rate] = rate;
                return sample;
            }).ToList()
        };

        result.CloneMetadataFrom(sound);
        result[NumericData.Rate] = rate;
        return result;
    }

    public Sound? Normalize(Sound sound, BigRational target, bool cutOnly)
    {
        var newSound = new Sound
        {
            Samples = [..sound.Samples]
        };

        var level = sound.Samples.Max(s =>
        {
            var max = 0f;
            foreach (var x in s.Data.Span)
                max = Math.Max(Math.Abs(x), max);
            return max;
        });

        if (level is > 0 and (< 1 or > 1) && (!cutOnly || level > 1))
        {
            var amp = (float)(target / level);
            foreach (var sample in newSound.Samples)
                ApplyGain(sample.Data.Span, amp);
        }

        newSound.CloneMetadataFrom(sound);
        return newSound;
    }

    public Sound IntegerDownsample(Sound sound, int factor)
    {
        var newSound = new Sound
        {
            Samples = sound.Samples.Select(s =>
            {
                var rate = s[NumericData.Rate] ?? sound[NumericData.Rate] ??
                    throw new RhythmCodexException("Can't downsample without a source rate.");
                var sample = new Sample();
                sample.CloneMetadataFrom(s);
                if (s[NumericData.Rate] != null)
                    s[NumericData.Rate] /= factor;
                var length = s.Data.Length / factor;
                var data = new float[length];
                sample.Data = data;
                var cursor = data.AsSpan();
                var offset = 0;
                for (var i = 0; i < length; i++)
                {
                    var buffer = data[offset++];
                    for (var j = 1; j < factor; j++)
                        buffer += data[offset++];
                    cursor[0] = buffer / factor;
                    cursor = cursor[1..];
                }

                return sample;
            }).ToList()
        };

        newSound.CloneMetadataFrom(sound);
        newSound[NumericData.Rate] /= factor;
        return newSound;
    }

    public Sound? ApplyEffects(Sound? sound)
    {
        if (!sound.Samples.Any())
            return null;

        var samples = new List<Sample>(sound.Samples);
        if (samples.Count == 1)
            samples.Add(samples[0]);

        var result = new Sound
        {
            Samples = samples.Select(s =>
            {
                var sample = new Sample
                {
                    Data = s.Data.ToArray()
                };
                sample.CloneMetadataFrom(s);
                ApplyEffectsInternal(sample);
                return sample;
            }).ToList()
        };

        result.CloneMetadataFrom(sound);
        ApplyEffectsInternal(result);
        return result;
    }

    private void ApplyEffectsInternal(Sound? sound)
    {
        if (sound[NumericData.Volume].HasValue)
        {
            foreach (var sample in sound.Samples)
                ApplyGain(sample.Data.Span, sound[NumericData.Volume].Value);
            sound[NumericData.Volume] = null;
        }

        if (sound[NumericData.Panning].HasValue)
        {
            var isLeftPanning = true;
            var left = BigRational.Sqrt(BigRational.One - sound[NumericData.Panning].Value);
            var right = BigRational.Sqrt(sound[NumericData.Panning].Value);
            foreach (var sample in sound.Samples)
            {
                ApplyGain(sample.Data.Span,
                    isLeftPanning ? left : right);
                isLeftPanning = !isLeftPanning;
            }

            sound[NumericData.Panning] = null;
        }
    }

    private void ApplyEffectsInternal(Sample sample)
    {
        if (sample[NumericData.Volume].HasValue)
        {
            ApplyGain(sample.Data.Span, sample[NumericData.Volume]!.Value);
            sample[NumericData.Volume] = null;
        }
    }

    private static void ApplyGain(Span<float> data, BigRational value)
    {
        if (value == BigRational.One)
            return;

        var amp = (float)value;
        for (var i = 0; i < data.Length; i++)
            data[i] *= amp;
    }
}