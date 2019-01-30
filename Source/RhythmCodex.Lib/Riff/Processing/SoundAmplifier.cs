using System;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Riff.Processing
{
    [Service]
    public class SoundAmplifier : ISoundAmplifier
    {
        public void Amplify(ISound sound, float volume, float panning)
        {
            if (panning < 0f)
                panning = 0f;
            else if (panning > 1f)
                panning = 1f;

            if ((sound.Samples.Count & 1) != 0)
                sound.Samples.Add(sound.Samples.Last().Clone());

            var isLeft = false;
            var leftAmplification = (float)(volume * Math.Sqrt(1 - panning));
            var rightAmplification = (float)(volume * Math.Sqrt(panning));

            foreach (var sample in sound.Samples)
            {
                for (var i = 0; i < sample.Data.Count; i++)
                    sample.Data[i] *= isLeft ? leftAmplification : rightAmplification;
                
                isLeft = !isLeft;
            }
        }
    }
}