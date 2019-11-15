using System;
using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Thirdparty.SincInterpolation
{
    // https://codefying.com/2015/06/07/linear-and-cubic-spline-interpolation/
    
    internal sealed class CubicSplineInterpolation:Interpolation
    {
        public CubicSplineInterpolation(double[] _x, double[] _y): base(_x, _y, true)
        {
            len = X.Length;
            if (len > 1)
            {
                
                Console.WriteLine("Successfully set abscissa and ordinate.");
                baseset = true;
                // make a copy of X as a list for later use
                lX = X.ToList();
            }
            else
                Console.WriteLine("Ensure x and y are the same length and have at least 2 elements. All x values must be unique. X-values must be in ascending order.");
        }
 
        public override double? Interpolate(double p)
        {
            if (baseset)
            {
                double? result = 0;
                int N = len - 1;
                 
                double[] h = X.Diff();
                double[] D = Y.Diff().Scale(h, false);
                double[] s = Enumerable.Repeat(3.0, D.Length).ToArray();
                double[] dD3 = D.Diff().Scale(s);
                double[] a = Y;
                 
                // generate tridiagonal system
                double[,] H = new double[N-1, N-1];
                double[] diagVals = new double[N-1];
                for (int i=1; i<N;i++)
                {
                    diagVals[i-1] = 2 * (h[i-1] + h[i]);
                }
 
                H = H.Diag(diagVals);
 
                // H can be null if non-square matrix is passed
                if (H != null)
                {
                    for (int i = 0; i < N - 2; i++)
                    {
                        H[i, i + 1] = h[i + 1];
                        H[i + 1, i] = h[i + 1];
                    }
 
                    double[] c = new double[N + 2];
                    c = Enumerable.Repeat(0.0, N + 1).ToArray();
 
                    // solve tridiagonal matrix
                    Tridiagonal le = new Tridiagonal();
                    double[] solution = le.Solve(H, dD3);                   
 
                    for (int i = 1; i < N;i++ )
                    {
                        c[i] = solution[i - 1];
                    }
 
                    double[] b = new double[N];
                    double[] d = new double[N];
                    for (int i = 0; i < N; i++ )
                    {
                        b[i] = D[i] - (h[i] * (c[i + 1] + 2.0 * c[i])) / 3.0;
                        d[i] = (c[i + 1] - c[i]) / (3.0 * h[i]);
                    }
 
                    double Rx;
                     
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
                    int iRx = lX.IndexOf(Rx);
 
                    if (iRx == -1)
                        return null;
 
                    if (iRx == len - 1 && X[iRx] == p)
                        return Y[len - 1];
 
                    if (iRx == 0)
                        return Y[0];
 
                    iRx = lX.IndexOf(Rx)-1;
                    Rx = p - X[iRx];
                    result = a[iRx] + Rx * (b[iRx] + Rx * (c[iRx] + Rx * d[iRx]));
 
                    return result;
                }
                else
                {
                    return null;
                }
 
                 
            }
            else
                return null;
        }
 
        private bool baseset = false;
        private int len;
        private List<double> lX;
    }
}