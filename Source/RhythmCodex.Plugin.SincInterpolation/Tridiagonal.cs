using System;

namespace RhythmCodex.Thirdparty.SincInterpolation
{
    // https://codefying.com/2015/06/07/linear-and-cubic-spline-interpolation/
    
    public sealed class Tridiagonal
    {
        public double[] Solve(double[,] matrix, double[] d)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int len = d.Length;
            // this must be a square matrix
            if (rows == cols && rows == len)
            {
 
                double[] b = new double[rows];
                double[] a = new double[rows];
                double[] c = new double[rows];
                
                // decompose the matrix into its tri-diagonal components
                for (int i = 0; i < rows; i++)
                {
                    for (int j=0; j<cols; j++)
                    {
                        if (i == j)
                            b[i] = matrix[i, j];
                        else if (i == (j - 1))
                            c[i] = matrix[i, j];
                        else if (i == (j + 1))
                            a[i] = matrix[i, j];
                    }
                }
                try
                {
                    c[0] = c[0] / b[0];
                    d[0] = d[0] / b[0];
 
                    for (int i = 1; i < len - 1; i++)
                    {
                        c[i] = c[i] / (b[i] - a[i] * c[i - 1]);
                        d[i] = (d[i] - a[i] * d[i - 1]) / (b[i] - a[i] * c[i - 1]);
                    }
                    d[len - 1] = (d[len - 1] - a[len - 1] * d[len - 2]) / (b[len - 1] - a[len - 1] * c[len - 2]);
 
                    // back-substitution step
                    for (int i = (len - 1); i-- > 0; )
                    {
                        d[i] = d[i] - c[i] * d[i + 1];
                    }
 
                    return d;
                }
                catch(DivideByZeroException)
                {
                    Console.WriteLine("Division by zero was attempted. Most likely reason is that ");
                    Console.WriteLine("the tridiagonal matrix condition ||(b*i)||>||(a*i)+(c*i)|| is not satisified.");
                    return null;
                }
            }
            else
            {
                Console.WriteLine("Error: the matrix must be square. The vector must be the same size as the matrix length.");
                return null;
            }
        }
    }
}