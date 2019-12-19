using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Providers;

namespace RhythmCodex.Sounds.Converters
{
    [Service]
    public class AudioDsp : IAudioDsp
    {
        public ISound ApplyPanVolume(ISound sound, BigRational volume, BigRational panning)
        {
            var newSound = new Sound
            {
                Samples = new List<ISample>(sound.Samples)
            };

            newSound.CloneMetadataFrom((Metadata) sound);
            newSound[NumericData.Volume] = volume;
            newSound[NumericData.Panning] = panning;

            ApplyEffectsInternal(newSound);
            return newSound;
        }

        public ISound ApplyResampling(ISound sound, IResampler resampler, BigRational rate)
        {
            if (sound == null || !sound.Samples.Any())
                return null;
            
            if (rate <= BigRational.Zero || sound[NumericData.Rate] == rate)
                return sound;

            var targetRate = (float) (double) rate;
            var samples = new List<ISample>(sound.Samples);
            var result = new Sound
            {
                Samples = samples.Select(s =>
                {
                    var sourceRate = (float) (double) (s[NumericData.Rate] ?? sound[NumericData.Rate]);
                    var sample = new Sample
                    {
                        Data = resampler.Resample(s.Data, sourceRate, targetRate)
                    };
                    sample.CloneMetadataFrom((Metadata) s);
                    sample[NumericData.Rate] = rate;
                    return (ISample) sample;
                }).ToList()
            };

            result.CloneMetadataFrom((Metadata) sound);
            result[NumericData.Rate] = rate;
            return result;
        }

        public ISound Normalize(ISound sound, BigRational target)
        {
            var newSound = new Sound
            {
                Samples = new List<ISample>(sound.Samples)
            };

            var level = sound.Samples.SelectMany(s => s.Data).Max(s => Math.Abs(s));
            if (level > 0 && level != 1)
            {
                var amp = (float) (target / level);
                foreach (var sample in newSound.Samples)
                    ApplyGain(sample.Data, amp);
            }

            newSound.CloneMetadataFrom((Metadata) sound);
            return newSound;
        }

        public ISound IntegerDownsample(ISound sound, int factor)
        {
            var newSound = new Sound
            {
                Samples = sound.Samples.Select(s =>
                {
                    var rate = s[NumericData.Rate] ?? sound[NumericData.Rate] ??
                               throw new RhythmCodexException("Can't downsample without a source rate.");
                    var sample = new Sample();
                    sample.CloneMetadataFrom((Metadata) s);
                    if (s[NumericData.Rate] != null)
                        s[NumericData.Rate] /= factor;
                    var length = s.Data.Count / 2;
                    sample.Data = new float[length];
                    var offset = 0;
                    for (var i = 0; i < length; i++)
                    {
                        var buffer = s.Data[offset++];
                        for (var j = 1; j < factor; j++)
                            buffer += s.Data[offset++];
                        sample.Data[i] = buffer / factor;
                    }
                    return sample;
                }).Cast<ISample>().ToList()
            };

            newSound.CloneMetadataFrom((Metadata) sound);
            newSound[NumericData.Rate] /= factor;
            return newSound;
        }

        public ISound ApplyEffects(ISound sound)
        {
            if (!sound.Samples.Any())
                return null;

            var samples = new List<ISample>(sound.Samples);
            if (samples.Count == 1)
                samples.Add(samples[0]);

            var result = new Sound
            {
                Samples = samples.Select(s =>
                {
                    var sample = new Sample
                    {
                        Data = new List<float>(s.Data)
                    };
                    sample.CloneMetadataFrom((Metadata) s);
                    ApplyEffectsInternal(sample);
                    return (ISample) sample;
                }).ToList()
            };

            result.CloneMetadataFrom((Metadata) sound);
            ApplyEffectsInternal(result);
            return result;
        }

        private void ApplyEffectsInternal(ISound sound)
        {
            if (sound[NumericData.Volume].HasValue)
            {
                foreach (var sample in sound.Samples)
                    ApplyGain(sample.Data, sound[NumericData.Volume].Value);
                sound[NumericData.Volume] = null;
            }

            if (sound[NumericData.Panning].HasValue)
            {
                var isLeftPanning = true;
                var left = BigRational.Sqrt(BigRational.One - sound[NumericData.Panning].Value);
                var right = BigRational.Sqrt(sound[NumericData.Panning].Value);
                foreach (var sample in sound.Samples)
                {
                    ApplyGain(sample.Data,
                        isLeftPanning ? left : right);
                    isLeftPanning = !isLeftPanning;
                }

                sound[NumericData.Panning] = null;
            }
        }

        private void ApplyEffectsInternal(ISample sample)
        {
            if (sample[NumericData.Volume].HasValue)
            {
                ApplyGain(sample.Data, sample[NumericData.Volume].Value);
                sample[NumericData.Volume] = null;
            }
        }

        private void ApplyGain(IList<float> data, BigRational value)
        {
            if (value == BigRational.One)
                return;

            var amp = (float) value;
            for (var i = 0; i < data.Count; i++)
                data[i] = data[i] * amp;
        }
    }
}