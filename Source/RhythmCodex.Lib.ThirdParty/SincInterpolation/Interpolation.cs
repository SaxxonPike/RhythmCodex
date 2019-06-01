using System.Linq;

namespace RhythmCodex.Thirdparty.SincInterpolation
{
    // https://codefying.com/2015/06/07/linear-and-cubic-spline-interpolation/
    
    internal  abstract class Interpolation: IInterpolate
    {
         
        public Interpolation(double[] _x, double[] _y)
        {
            int xLength = _x.Length;
            if (xLength == _y.Length && xLength > 1 && _x.Distinct().Count() == xLength)
            {
                x = _x;
                y = _y;
            }
        }
 
        // cubic spline relies on the abscissa values to be sorted
        public Interpolation(double[] _x, double[] _y, bool checkSorted=true)
        {
            int xLength = _x.Length;
            if (checkSorted)
            {
                if (xLength == _y.Length && xLength > 1 && _x.Distinct().Count() == xLength && Enumerable.SequenceEqual(_x.SortedList(),_x.ToList()))
                {
                    x = _x;
                    y = _y;
                }
            }
            else
            {
                if (xLength == _y.Length && xLength > 1 && _x.Distinct().Count() == xLength)
                {
                    x = _x;
                    y = _y;
                }
            }
        }
 
        public double[] X
        {
            get
            {
                return x;
            }
        }
 
        public double[] Y
        {
            get
            {
                return y;
            }
        }
 
        public abstract double? Interpolate(double p);
 
        private double[] x;
        private double[] y;
    }
}