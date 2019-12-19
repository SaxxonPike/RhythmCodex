using System;
using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Plugin.SincInterpolation
{
    // https://codefying.com/2015/06/07/linear-and-cubic-spline-interpolation/

    internal static class Extensions
    {
        // returns a list sorted in ascending order 
        public static IList<T> SortedList<T>(this IList<T> array)
        {
            var l = array.ToList();
            l.Sort();
            return l;
        }

        // returns a difference between consecutive elements of an array
        public static IList<float> Diff(this IList<float> array)
        {
            var len = array.Count - 1;
            var diffsArray = new float[len];
            for (var i = 1; i <= len; i++)
            {
                diffsArray[i - 1] = array[i] - array[i - 1];
            }

            return diffsArray;
        }

        // scaled an array by another array of doubles
        public static IList<float> Scale(this IList<float> array, IList<float> scalor, bool mult = true)
        {
            var len = array.Count;
            var scaledArray = new float[len];

            if (mult)
            {
                for (var i = 0; i < len; i++)
                {
                    scaledArray[i] = array[i] * scalor[i];
                }
            }
            else
            {
                for (var i = 0; i < len; i++)
                {
                    if (scalor[i] != 0)
                    {
                        scaledArray[i] = array[i] / scalor[i];
                    }
                    else
                    {
                        // basic fix to prevent division by zero
                        scalor[i] = 0.00001f;
                        scaledArray[i] = array[i] / scalor[i];
                    }
                }
            }

            return scaledArray;
        }

        public static float[,] Diag(this float[,] matrix, IList<float> diagVals)
        {
            var rows = matrix.GetLength(0);
            var cols = matrix.GetLength(1);
            // the matrix has to be square
            if (rows == cols)
            {
                var diagMatrix = new float[rows, cols];
                var k = 0;
                for (var i = 0; i < rows; i++)
                for (var j = 0; j < cols; j++)
                {
                    if (i == j)
                    {
                        diagMatrix[i, j] = diagVals[k];
                        k++;
                    }
                    else
                    {
                        diagMatrix[i, j] = 0;
                    }
                }

                return diagMatrix;
            }

            throw new Exception("Diag should be used on square matrix only.");
        }
    }
}