using System;
using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Thirdparty.SincInterpolation
{
    // https://codefying.com/2015/06/07/linear-and-cubic-spline-interpolation/
    
    internal sealed class LinearInterpolation: Interpolation
    {
         
        public LinearInterpolation(double[] _x, double [] _y):base(_x,_y)
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
                Console.WriteLine("Ensure x and y are the same length and have at least 2 elements. All x values must be unique.");
        }
 
        public override double? Interpolate(double p)
        {
            if (baseset)
            {
                double? result = null;
                double Rx;
 
                try
                {
                    // point p may be outside abscissa's range
                    // if it is, we return null
                    Rx = X.First(s => s >= p);
                }
                catch(ArgumentNullException)
                {
                    return null;
                }
 
                // at this stage we know that Rx contains a valid value
                // find the index of the value close to the point required to be interpolated for
                int i = lX.IndexOf(Rx);
 
                // provide for index not found and lower and upper tabulated bounds
                if (i==-1)
                    return null;
                         
                if (i== len-1 && X[i]==p)
                    return Y[len - 1];
 
                if (i == 0)
                    return Y[0];
 
                // linearly interpolate between two adjacent points
                double h = (X[i] - X[i - 1]);
                double A = (X[i] - p) / h;
                double B = (p - X[i - 1]) / h;
 
                result = Y[i - 1] * A + Y[i] * B;
 
                return result;
                 
            }
            else
            {
                return null;
            }
        }
 
        private bool baseset = false;
        private int len;
        private List<double> lX;
    }
}