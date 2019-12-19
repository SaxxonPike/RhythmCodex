using System;
using System.Collections.Generic;

namespace RhythmCodex.Plugin.SincInterpolation
{
    // https://codefying.com/2015/06/07/linear-and-cubic-spline-interpolation/

    internal static class Tridiagonal
    {
        public static IList<float> Solve(float[,] matrix, IList<float> d)
        {
            var rows = matrix.GetLength(0);
            var cols = matrix.GetLength(1);
            var len = d.Count;
            // this must be a square matrix
            if (rows == cols && rows == len)
            {
                var b = new float[rows];
                var a = new float[rows];
                var c = new float[rows];

                // decompose the matrix into its tri-diagonal components
                for (var i = 0; i < rows; i++)
                {
                    for (var j = 0; j < cols; j++)
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

                    for (var i = 1; i < len - 1; i++)
                    {
                        c[i] = c[i] / (b[i] - a[i] * c[i - 1]);
                        d[i] = (d[i] - a[i] * d[i - 1]) / (b[i] - a[i] * c[i - 1]);
                    }

                    d[len - 1] = (d[len - 1] - a[len - 1] * d[len - 2]) / (b[len - 1] - a[len - 1] * c[len - 2]);

                    // back-substitution step
                    for (var i = (len - 1); i-- > 0;)
                    {
                        d[i] = d[i] - c[i] * d[i + 1];
                    }

                    return d;
                }
                catch (DivideByZeroException e)
                {
                    throw new Exception("Division by zero was attempted. Most likely reason is that " +
                                        "the tridiagonal matrix condition ||(b*i)||>||(a*i)+(c*i)||" +
                                        "is not satisified.", e);
                }
            }

            throw new Exception(
                "Error: the matrix must be square. The vector must be the same size as the matrix length.");
        }
    }
}