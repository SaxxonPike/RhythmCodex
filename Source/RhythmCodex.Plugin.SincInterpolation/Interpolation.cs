using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Providers;

namespace RhythmCodex.Plugin.SincInterpolation
{
    // https://codefying.com/2015/06/07/linear-and-cubic-spline-interpolation/

    public interface IInterpolation
    {
        float? Interpolate(float p);
    }
    
    internal abstract class Interpolation : IInterpolation
    {
        protected Interpolation(IList<float> x, IList<float> y)
        {
            var xLength = x.Count;
            if (xLength != y.Count || xLength <= 1 || x.Distinct().Count() != xLength)
                return;

            X = x;
            Y = y;
        }

        // cubic spline relies on the abscissa values to be sorted
        protected Interpolation(IList<float> x, IList<float> y, bool checkSorted = true)
        {
            var xLength = x.Count;
            if (checkSorted)
            {
                if (xLength != y.Count || xLength <= 1 || x.Distinct().Count() != xLength ||
                    !x.SortedList().SequenceEqual(x.ToList()))
                    return;

                X = x;
                Y = y;
            }
            else
            {
                if (xLength != y.Count || xLength <= 1 || x.Distinct().Count() != xLength)
                    return;

                X = x;
                Y = y;
            }
        }

        protected IList<float> X { get; }

        protected IList<float> Y { get; }

        public abstract float? Interpolate(float p);
        
        public abstract string Name { get; }
    }
}