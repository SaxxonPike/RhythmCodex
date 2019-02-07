using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Dsp
{
    [Service]
    public class AudioDsp : IAudioDsp
    {
        public ISound ApplyResampling(ISound sound, BigRational rate)
        {
            // Garbage non-interpolated resampling? Check.
            if (rate <= BigRational.Zero || sound[NumericData.Rate] == rate)
                return sound;

            if (rate < 8000)
            {
                
            }

            var samples = new List<ISample>(sound.Samples);

            var result = new Sound
            {
                Samples = samples.Select(s =>
                {
                    var interval = rate / sound[NumericData.Rate].Value;
                    var targetSize = (int) (interval * s.Data.Count + interval);
                    var data = new float[targetSize];
                    var sourceData = s.Data;
                    var targetOffset = BigRational.Zero;
                    var targetPointer = 0;

                    for (var sourceOffset = 0; sourceOffset < sourceData.Count; sourceOffset++)
                    {
                        targetOffset += interval;
                        while (targetPointer < targetOffset)
                            data[targetPointer++] = sourceData[sourceOffset];
                    }

                    var sample = new Sample
                    {
                        Data = data
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
            var amp = (double) value;
            for (var i = 0; i < data.Count; i++)
                data[i] = (float) (data[i] * amp);
        }
    }
}