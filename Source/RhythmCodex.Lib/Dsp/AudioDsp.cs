using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Dsp
{
    [Service]
    public class AudioDsp : IAudioDsp
    {
        public ISound ApplyResampling(ISound sound, int rate)
        {
            throw new NotImplementedException();
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