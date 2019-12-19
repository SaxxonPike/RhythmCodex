using System;
using System.Collections.Generic;
using RhythmCodex.Sounds.Providers;

namespace RhythmCodex.Plugin.SincInterpolation
{
    public abstract class BaseResampler : IResampler
    {
        protected abstract IInterpolation GetInterpolation(IList<float> x, IList<float> y);
        
        public abstract string Name { get; }
        public abstract int Priority { get; }

        public IList<float> Resample(IList<float> data, float sourceRate, float targetRate)
        {
            
            var inputLength = data.Count;
            var ex = new float[inputLength];
            var ey = new float[inputLength];
            for (var i = 0; i < inputLength; i++)
            {
                ex[i] = i;
                ey[i] = data[i];
            }
            
            var interp = GetInterpolation(ex, ey);
            var outputLength = (int) Math.Round(inputLength * targetRate / sourceRate);
            var output = new List<float>(outputLength);
            for (var i = 0; i < outputLength; i++)
                output.Add(interp.Interpolate(i * targetRate / sourceRate) ?? 0);

            return output;
        }
    }
}