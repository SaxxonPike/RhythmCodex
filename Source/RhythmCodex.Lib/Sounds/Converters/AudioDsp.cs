using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Resampler.Providers;

namespace RhythmCodex.Sounds.Converters;

[Service]
public class AudioDsp : IAudioDsp
{
    private static readonly BigRational Sqrt2 = BigRational.Sqrt(2);

    public Sound Mix(IEnumerable<Sound> sounds)
    {
        var bounced = sounds.Select(ApplyEffects).ToList();
        var length = bounced.Max(b => b.Samples.Count > 0 ? b.Samples.Max(s => s.Data.Length) : 0);
        var channels = bounced.Max(b => b.Samples.Count);

        using var builder = new SoundBuilder(channels, length);
        builder.CloneMetadataFrom(bounced.First());
        builder[NumericData.Panning] = null;
        builder[NumericData.Volume] = null;

        foreach (var bounce in bounced)
        {
            for (var c = 0; c < channels; c++)
            {
                if (bounce == null)
                    continue;

                AudioSimd.Mix(builder.Samples[c].AsSpan(), bounce.Samples[c].Data.Span);
            }
        }

        return builder.ToSound();
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
                    MemoryMarshal.Cast<byte, short>(output.AsSpan()),
                    sound.Samples[i].Data.Span,
                    i,
                    sampleCount
                );
        });

        return output;
    }

    public Sample[] FloatsToSamples(ReadOnlySpan<float> inFloats, int channels)
    {
        var inputLength = inFloats.Length;

        switch (channels)
        {
            // invalid
            case < 1:
            {
                return [];
            }
            // mono
            case 1:
            {
                return
                [
                    new Sample
                    {
                        Data = inFloats.ToArray()
                    }
                ];
            }
            // stereo (fast)
            case 2:
            {
                var left = new float[inFloats.Length / channels];
                var right = new float[left.Length];

                var samples = new Sample[]
                {
                    new() { Data = left },
                    new() { Data = right }
                };

                AudioSimd.Deinterleave2(inFloats, left, right);

                return samples;
            }
            // all others (interleaved)
            default:
            {
                var samples = new float[channels][];
                for (var c = 0; c < channels; c++)
                    samples[c] = new float[inFloats.Length / channels];

                for (int i = 0, j = 0; i < inputLength; j++)
                {
                    for (var c = 0; c < channels; c++)
                        samples[c][j] = inFloats[i++];
                }

                return samples.Select(s => new Sample
                {
                    Data = s
                }).ToArray();
            }
        }
    }

    public Sound ApplyResampling(Sound sound, IResampler resampler, BigRational rate)
    {
        if (sound.Samples.Count < 1)
        {
            var emptyResult = sound.CloneWithSamples([]);
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

        var result = sound.CloneWithSamples(sound.Samples.Select(s =>
        {
            var sourceRate = (float)(double)(s[NumericData.Rate] ?? sound[NumericData.Rate])!;
            var sample = s.CloneWithData(resampler.Resample(s.Data.Span, sourceRate, targetRate));
            sample[NumericData.Rate] = rate;
            return sample;
        }));

        result[NumericData.Rate] = rate;
        return result;
    }

    private static void ApplyGain(SoundBuilder sound, float amp)
    {
        foreach (var sample in sound.Samples)
            AudioSimd.Gain(sample.AsSpan(), amp);
    }

    public Sound Normalize(Sound sound, BigRational target, bool cutOnly)
    {
        var level = sound.Samples.Max(s =>
        {
            var max = 0f;
            foreach (var x in s.Data.Span)
                max = Math.Max(Math.Abs(x), max);
            return max;
        });

        if (level is <= 0 or >= 1 and <= 1 || (cutOnly && !(level > 1)))
            return sound;

        using var newSound = SoundBuilder.FromSound(sound);
        var amp = (float)(target / level);
        ApplyGain(newSound, amp);
        return newSound.ToSound();
    }

    public Sound IntegerDownsample(Sound sound, int factor)
    {
        var newSound = sound.CloneWithSamples(sound.Samples.Select(s =>
        {
            var rate = s[NumericData.Rate] ?? sound[NumericData.Rate] ??
                throw new RhythmCodexException("Can't downsample without a source rate.");
            var length = s.Data.Length / factor;
            var data = new float[length];
            var sample = s.CloneWithData(data);
            sample[NumericData.Rate] = rate / factor;
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
        }));

        newSound.CloneMetadataFrom(sound);
        newSound[NumericData.Rate] /= factor;
        return newSound;
    }

    public Sound ApplyEffects(Sound sound)
    {
        ArgumentNullException.ThrowIfNull(sound);

        if (sound.Samples.Count == 0)
            return sound;

        var mixdown = sound;
        
        if (sound.Mixer != null)
            mixdown = sound.Mixer().MixDown(sound, null) ?? mixdown;

        var builder = SoundBuilder.FromSound(mixdown, Math.Max(mixdown.Samples.Count, 2));
        ApplyEffectsInternal(builder);
        return builder.ToSound();

        return mixdown;
    }

    private static void ApplyEffectsInternal(SoundBuilder sound)
    {
        foreach (var s in sound.Samples)
            ApplyEffectsInternal(s);

        if (sound[NumericData.Volume] is { } volume)
        {
            if (volume != BigRational.One)
            {
                foreach (var sample in sound.Samples)
                {
                    if (volume == BigRational.Zero)
                        sample.Fill(0);
                    else
                        AudioSimd.Gain(sample.AsSpan(), (float)volume);
                }
            }

            sound[NumericData.Volume] = null;
        }

        if (sound[NumericData.Panning] is { } panning)
        {
            var isLeftPanning = true;

            //
            // Circular panning will cause hard pans to be 1.0, but center to be 1/sqrt(2).
            // We want center panned things to be at unity, so multiplying by sqrt(2) should
            // fix that. This will however cause hard panned things to exceed 1.0 as far as
            // gain.
            //

            var left = (float)(BigRational.Sqrt(BigRational.One - panning) * Sqrt2);
            var right = (float)(BigRational.Sqrt(panning) * Sqrt2);

            foreach (var sample in sound.Samples)
            {
                var channelPan = isLeftPanning ? left : right;

                if (channelPan == BigRational.Zero)
                    sample.Fill(0);
                else if (channelPan != BigRational.One)
                    AudioSimd.Gain(sample.AsSpan(), isLeftPanning ? left : right);

                isLeftPanning = !isLeftPanning;
            }

            sound[NumericData.Panning] = null;
        }
    }

    private static void ApplyEffectsInternal(SampleBuilder sample)
    {
        if (sample[NumericData.Volume] is { } volume)
        {
            if (volume == BigRational.Zero)
                sample.Fill(0);
            else if (volume != BigRational.One)
                AudioSimd.Gain(sample.AsSpan(), (float)volume);

            sample[NumericData.Volume] = null;
        }
    }
}