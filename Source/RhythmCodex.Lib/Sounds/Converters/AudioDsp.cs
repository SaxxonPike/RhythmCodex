using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Resampler.Providers;

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

                AudioSimd.Mix(newSound.Samples[c].Data.Span, bounce.Samples[c].Data.Span);
            }
        }

        return newSound;
    }

    public byte[] Interleave16Bits(Sound sound)
    {
        var sampleCount = sound.Samples.Count;
        if (sampleCount == 0)
            return [];

        var maxSampleLength = sound.Samples.Max(s => s.Data.Length);
        var output = new byte[sampleCount * maxSampleLength * 2];

        Parallel.ForEach(Partitioner.Create(0, sound.Samples.Count), range =>
        {
            for (var i = range.Item1; i < range.Item2; i++)
                AudioSimd.Quantize16(
                    sound.Samples[i].Data.Span,
                    MemoryMarshal.Cast<byte, short>(output.AsSpan()),
                    i,
                    sampleCount
                );
        });

        // for (var i = 0; i < sampleCount; i++)
        // {
        //     AudioSimd.Quantize16(
        //         sound.Samples[i].Data.Span,
        //         MemoryMarshal.Cast<byte, short>(output.AsSpan()),
        //         i,
        //         sampleCount
        //     );
        // }
        
        return output;
    }

    public Sound ApplyPanVolume(Sound sound, BigRational volume, BigRational panning)
    {
        if (sound.Samples.Count < 1)
        {
            var emptyResult = new Sound();
            emptyResult.CloneMetadataFrom(sound);
            return emptyResult;
        }

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

    public Sound ApplyResampling(Sound sound, IResampler resampler, BigRational rate)
    {
        if (sound.Samples.Count < 1)
        {
            var emptyResult = new Sound();
            emptyResult.CloneMetadataFrom(sound);
            return emptyResult;
        }

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
                AudioSimd.Gain(sample.Data.Span, amp);
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
                AudioSimd.Gain(sample.Data.Span, amp);
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

    public Sound ApplyEffects(Sound sound)
    {
        ArgumentNullException.ThrowIfNull(sound, nameof(sound));

        if (sound.Samples.Count == 0)
            return sound;

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
                AudioSimd.Gain(sample.Data.Span, sound[NumericData.Volume]!.Value);

            sound[NumericData.Volume] = null;
        }

        if (sound[NumericData.Panning].HasValue)
        {
            var isLeftPanning = true;
            var left = BigRational.Sqrt(BigRational.One - sound[NumericData.Panning]!.Value);
            var right = BigRational.Sqrt(sound[NumericData.Panning]!.Value);
            foreach (var sample in sound.Samples)
            {
                AudioSimd.Gain(sample.Data.Span,
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
            AudioSimd.Gain(sample.Data.Span, sample[NumericData.Volume]!.Value);
            sample[NumericData.Volume] = null;
        }
    }
}