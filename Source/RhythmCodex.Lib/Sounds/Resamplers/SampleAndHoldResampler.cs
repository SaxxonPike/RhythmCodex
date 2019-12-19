using System.Collections.Generic;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Providers;

namespace RhythmCodex.Sounds.Resamplers
{
    [Service]
    public class SampleAndHoldResampler : IResampler
    {
        public string Name => "sampleandhold";

        public int Priority => int.MinValue;

        public IList<float> Resample(IList<float> data, float sourceRate, float targetRate)
        {
            var accumulator = 0f;
            var targetSize = (int) ((data.Count * targetRate + (sourceRate - 1)) / sourceRate);
            var result = new float[targetSize];
            var sourceData = data;
            var sourceCounter = 0;
            var targetCounter = 0;

            while (targetCounter < targetSize)
            {
                while (accumulator > targetRate)
                {
                    accumulator -= targetRate;
                    sourceCounter++;
                }

                result[targetCounter++] = sourceData[sourceCounter];
                accumulator += sourceRate;
            }

            return result;
        }
    }
}