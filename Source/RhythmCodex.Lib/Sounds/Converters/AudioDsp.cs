using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Providers;

namespace RhythmCodex.Sounds.Converters;

[Service]
public class AudioDsp : IAudioDsp
{
    public Sound Mix(IEnumerable<Sound> sounds)
    {
        var bounced = sounds.Select(ApplyEffects).ToList();
        var length = bounced.Max(b => b.Samples.Count > 0 ? b.Samples.Max(s => s.Data.Length) : 0);
        var channels = bounced.Max(b => b.Samples.Count);

        var newSound = new Sound
        {
            Samples = Enumerable.Range(0, channels).Select(_ => new Sample
            {
                Data = new float[length]
            }).ToList()
        };

        newSound.CloneMetadataFrom(bounced.First());
        newSound[NumericData.Panning] = null;
        newSound[NumericData.Volume] = null;

        foreach (var bounce in bounced)
        {
            for (var c = 0; c < channels; c++)
            {
                if (bounce == null)
                    continue;

                var srcSpan = bounce.Samples[c].Data.Span;
                var dstSpan = newSound.Samples[c].Data.Span;
                for (var i = 0; i < bounce.Samples[c].Data.Length; i++)
                    dstSpan[i] += srcSpan[i];
            }
        }

        return newSound;
    }

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
        if (sound == null || sound.Samples.Count == 0)
            return null;

        if (rate <= BigRational.Zero ||
            sound[NumericData.Rate] == rate ||
            sound[NumericData.Rate] == 0 ||
            sound.Samples == null! ||
            sound.Samples.Count == 0 ||
            sound.Samples.Any(sa => sa[NumericData.Rate] != null && sa[NumericData.Rate] <= 0))
            return sound;

        var targetRate = (float)(double)rate;
        var samples = new List<Sample>(sound.Samples);
        var result = new Sound
        {
            Samples = samples.Select(s =>
            {
                var sourceRate = (float)(double)(s[NumericData.Rate] ?? sound[NumericData.Rate])!;
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

    public Sound Normalize(Sound sound, BigRational target, bool cutOnly)
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

    public void Normalize(IEnumerable<Sound> sounds, BigRational target, bool cutOnly)
    {
        var soundList = sounds.AsList();

        var level = soundList.AsParallel().Select(sound =>
            sound.Samples.DefaultIfEmpty().Max(s =>
            {
                var max = 0f;
                foreach (var x in s.Data.Span)
                    max = Math.Max(Math.Abs(x), max);
                return max;
            })).DefaultIfEmpty().Max();

        if (level > 0 && level != 1 && (!cutOnly || level > 1))
        {
            var amp = (float)(target / level);
                
            foreach (var sample in soundList.SelectMany(sound => sound.Samples).Distinct().AsParallel())
                ApplyGain(sample.Data.Span, amp);
        }
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
                s[NumericData.Rate] = rate / factor;
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
        if (sound == null || sound.Samples.Count == 0)
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

    private static void ApplyEffectsInternal(Sound? sound)
    {
        if (sound == null)
            return;
        
        if (sound[NumericData.Volume].HasValue)
        {
            foreach (var sample in sound.Samples)
                ApplyGain(sample.Data.Span, sound[NumericData.Volume]!.Value);
            sound[NumericData.Volume] = null;
        }

        if (sound[NumericData.Panning].HasValue)
        {
            var isLeftPanning = true;
            var left = BigRational.Sqrt(BigRational.One - sound[NumericData.Panning]!.Value);
            var right = BigRational.Sqrt(sound[NumericData.Panning]!.Value);
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
        var cursor = data;

        if (Vector512.IsHardwareAccelerated)
        {
            while (cursor.Length >= 16)
            {
                var v = Vector512.Create<float>(cursor) * amp;
                v.CopyTo(cursor);
                cursor = cursor[16..];
            }
        }
        else if (Vector256.IsHardwareAccelerated)
        {
            while (cursor.Length >= 8)
            {
                var v = Vector256.Create<float>(cursor) * amp;
                v.CopyTo(cursor);
                cursor = cursor[8..];
            }
        }
        else if (Vector128.IsHardwareAccelerated)
        {
            while (cursor.Length >= 4)
            {
                var v = Vector128.Create<float>(cursor) * amp;
                v.CopyTo(cursor);
                cursor = cursor[4..];
            }
        }
        else if (Vector64.IsHardwareAccelerated)
        {
            while (cursor.Length >= 2)
            {
                var v = Vector64.Create<float>(cursor) * amp;
                v.CopyTo(cursor);
                cursor = cursor[2..];
            }
        }

        for (var i = 0; i < cursor.Length; i++)
            cursor[i] *= amp;
    }
}