using System;
using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Plugin.SincInterpolation
{
    // https://codefying.com/2015/06/07/linear-and-cubic-spline-interpolation/

    internal sealed class LinearInterpolation : Interpolation
    {
        public LinearInterpolation(IList<float> x, IList<float> y) : base(x, y)
        {
            _len = X.Count;
            if (_len > 1)
            {
                _baseset = true;
                // make a copy of X as a list for later use
                _lX = X.ToList();
            }
            else
                throw new Exception(
                    "Ensure x and y are the same length and have at least 2 elements. All x values must be unique.");
        }

        public override float? Interpolate(float p)
        {
            if (_baseset)
            {
                float rx;

                try
                {
                    // point p may be outside abscissa's range
                    // if it is, we return null
                    rx = X.First(s => s >= p);
                }
                catch (ArgumentNullException)
                {
                    return null;
                }

                // at this stage we know that Rx contains a valid value
                // find the index of the value close to the point required to be interpolated for
                var i = _lX.IndexOf(rx);

                // provide for index not found and lower and upper tabulated bounds
                if (i == -1)
                    return null;

                if (i == _len - 1 && X[i] == p)
                    return Y[_len - 1];

                if (i == 0)
                    return Y[0];

                // linearly interpolate between two adjacent points
                var h = (X[i] - X[i - 1]);
                var a = (X[i] - p) / h;
                var b = (p - X[i - 1]) / h;

                var result = Y[i - 1] * a + Y[i] * b;
                return result;
            }
            else
            {
                return null;
            }
        }

        public override string Name => "linear";

        private readonly bool _baseset;
        private readonly int _len;
        private readonly List<float> _lX;
    }
}