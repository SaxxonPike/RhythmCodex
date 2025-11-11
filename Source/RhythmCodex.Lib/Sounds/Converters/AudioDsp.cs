using System;
using System.Buffers.Binary;
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

        return output;
    }

    public Sample[] BytesToSamples(ReadOnlySpan<byte> inData, int bitsPerSample, int channels, bool bigEndian)
    {
        var inputLength = inData.Length;
        float[] inFloats;

        if (channels < 1 || bitsPerSample < 8)
            return [];

        switch (bitsPerSample)
        {
            // 8 bit
            case 8:
            {
                inFloats = new float[inputLength];
                for (var i = 0; i < inputLength; i++)
                    inFloats[i] = inData[i] / 255f;
                break;
            }
            // 16 bit
            case 16:
            {
                inFloats = new float[inputLength / 2];
                inputLength = inputLength / 2 * 2;
                if (!bigEndian)
                {
                    for (int i = 0, j = 0; i < inputLength; i += 2, j++)
                        inFloats[j] = BinaryPrimitives.ReadInt16LittleEndian(inData[i..]) / 32768f;
                }
                else
                {
                    for (int i = 0, j = 0; i < inputLength; i += 2, j++)
                        inFloats[j] = BinaryPrimitives.ReadInt16BigEndian(inData[i..]) / 32768f;
                }

                break;
            }
            // 24 bit
            case 24:
            {
                inFloats = new float[inputLength / 3];
                inputLength = inputLength / 3 * 3;
                if (!bigEndian)
                {
                    for (int i = 0, j = 0; i < inputLength; i += 3, j++)
                        inFloats[j] = (inData[0] | (inData[1] << 8) | (inData[2] << 16)) / 16777215f;
                }
                else
                {
                    for (int i = 0, j = 0; i < inputLength; i += 3, j++)
                        inFloats[j] = (inData[2] | (inData[1] << 8) | (inData[0] << 16)) / 16777215f;
                }

                break;
            }
            // 32 bit
            case 32:
            {
                inFloats = new float[inputLength / 4];
                inputLength = inputLength / 4 * 4;
                if (!bigEndian)
                {
                    for (int i = 0, j = 0; i < inputLength; i += 4, j++)
                        inFloats[j] = BinaryPrimitives.ReadInt32LittleEndian(inData[i..]) / (float)int.MaxValue;
                }
                else
                {
                    for (int i = 0, j = 0; i < inputLength; i += 4, j++)
                        inFloats[j] = BinaryPrimitives.ReadInt32BigEndian(inData[i..]) / (float)int.MaxValue;
                }

                break;
            }
            // unsupported
            default:
            {
                return [];
            }
        }

        if (channels == 1)
        {
            return
            [
                new Sample
                {
                    Data = inFloats
                }
            ];
        }

        return FloatsToSamples(inFloats, channels);
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
                var samples = new Sample[]
                {
                    new() { Data = new float[inFloats.Length / channels] },
                    new() { Data = new float[inFloats.Length / channels] }
                };

                AudioSimd.Deinterleave2(inFloats, samples[0].Data.Span, samples[1].Data.Span);

                return samples;
            }
            // all others (interleaved)
            default:
            {
                var samples = new Sample[channels];

                for (var c = 0; c < channels; c++)
                    samples[c] = new Sample { Data = new float[inFloats.Length / channels] };

                for (int i = 0, j = 0; i < inputLength; j++)
                {
                    for (var c = 0; c < channels; c++)
                        samples[c].Data.Span[j] = inFloats[i++];
                }

                return samples;
            }
        }
    }

    public (float[] A, float[] B) Deinterleave2(
        Span<float> data
    )
    {
        var outLength = data.Length / 2;
        var result = (A: new float[outLength], B: new float[outLength]);
        AudioSimd.Deinterleave2(data, result.A, result.B);
        return result;
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
                if (s?.Data is null)
                    return 0;

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

        if (sound[NumericData.Volume] is {} volume)
        {
            if (volume != BigRational.One)
            {
                foreach (var sample in sound.Samples)
                {
                    if (volume == BigRational.Zero)
                        sample.Data.Span.Clear();
                    else
                        AudioSimd.Gain(sample.Data.Span, volume);
                }
            }

            sound[NumericData.Volume] = null;
        }

        if (sound[NumericData.Panning] is {} panning)
        {
            if (panning != BigRational.OneHalf)
            {
                var isLeftPanning = true;
                var left = BigRational.Sqrt(BigRational.One - panning);
                var right = BigRational.Sqrt(panning);

                foreach (var sample in sound.Samples)
                {
                    var channelPan = isLeftPanning ? left : right;
                    
                    if (channelPan == BigRational.Zero)
                        sample.Data.Span.Clear();
                    else if (channelPan != BigRational.One)
                        AudioSimd.Gain(sample.Data.Span, isLeftPanning ? left : right);

                    isLeftPanning = !isLeftPanning;
                }
            }

            sound[NumericData.Panning] = null;
        }
    }

    private void ApplyEffectsInternal(Sample sample)
    {
        if (sample[NumericData.Volume] is {} volume)
        {
            if (volume == BigRational.Zero)
                sample.Data.Span.Clear();
            else if (volume != BigRational.One)
                AudioSimd.Gain(sample.Data.Span, volume);

            sample[NumericData.Volume] = null;
        }
    }
}