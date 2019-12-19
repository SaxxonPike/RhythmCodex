using System;
using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Plugin.SincInterpolation
{
    // https://codefying.com/2015/06/07/linear-and-cubic-spline-interpolation/

    internal sealed class CubicSplineInterpolation : Interpolation
    {
        public CubicSplineInterpolation(IList<float> x, IList<float> y) : base(x, y, true)
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
                    "Ensure x and y are the same length and have at least 2 elements. All x values must be unique. " +
                    "X-values must be in ascending order.");
        }

        public override float? Interpolate(float p)
        {
            if (_baseset)
            {
                var N = _len - 1;

                var h = X.Diff();
                var D = Y.Diff().Scale(h, false);
                var s = Enumerable.Repeat(3.0f, D.Count).ToArray();
                var dD3 = D.Diff().Scale(s);
                var a = Y;

                // generate tridiagonal system
                var H = new float[N - 1, N - 1];
                var diagVals = new float[N - 1];
                for (var i = 1; i < N; i++)
                {
                    diagVals[i - 1] = 2 * (h[i - 1] + h[i]);
                }

                H = H.Diag(diagVals);

                // H can be null if non-square matrix is passed
                if (H != null)
                {
                    for (var i = 0; i < N - 2; i++)
                    {
                        H[i, i + 1] = h[i + 1];
                        H[i + 1, i] = h[i + 1];
                    }

                    var c = Enumerable.Repeat(0.0f, N + 1).ToArray();

                    // solve tridiagonal matrix
                    var solution = Tridiagonal.Solve(H, dD3);

                    for (var i = 1; i < N; i++)
                    {
                        c[i] = solution[i - 1];
                    }

                    var b = new float[N];
                    var d = new float[N];
                    for (var i = 0; i < N; i++)
                    {
                        b[i] = D[i] - (h[i] * (c[i + 1] + 2.0f * c[i])) / 3.0f;
                        d[i] = (c[i + 1] - c[i]) / (3.0f * h[i]);
                    }

                    float Rx;

                    try
                    {
                        // point p may be outside abscissa's range
                        // if it is, we return null
                        Rx = X.First(m => m >= p);
                    }
                    catch
                    {
                        return null;
                    }

                    // at this stage we know that Rx contains a valid value
                    // find the index of the value close to the point required to be interpolated for
                    var iRx = _lX.IndexOf(Rx);

                    if (iRx == -1)
                        return null;

                    if (iRx == _len - 1 && X[iRx] == p)
                        return Y[_len - 1];

                    if (iRx == 0)
                        return Y[0];

                    iRx = _lX.IndexOf(Rx) - 1;
                    Rx = p - X[iRx];

                    var result = a[iRx] + Rx * (b[iRx] + Rx * (c[iRx] + Rx * d[iRx]));
                    return result;
                }

                return null;
            }

            return null;
        }

        public override string Name => "cubic";

        private readonly bool _baseset;
        private readonly int _len;
        private readonly List<float> _lX;
    }
}