using System;
using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Thirdparty.SincInterpolation
{
    // https://codefying.com/2015/06/07/linear-and-cubic-spline-interpolation/
    
    public static class Extensions
    {
        // returns a list sorted in ascending order 
        public static List<T> SortedList<T>(this T[] array)
        {
            List<T> l = array.ToList();
            l.Sort();
            return l;
 
        }
 
        // returns a difference between consecutive elements of an array
        public static double[] Diff(this double[] array)
        {
            int len = array.Length - 1;
            double[] diffsArray = new double[len];
            for (int i = 1; i <= len;i++)
            {
                diffsArray[i - 1] = array[i] - array[i - 1];
            }
            return diffsArray;
        }
 
        // scaled an array by another array of doubles
        public static double[] Scale(this double[] array, double[] scalor, bool mult=true)
        {
            int len = array.Length;
            double[] scaledArray = new double[len];
             
            if (mult)
            {
                for (int i = 0; i < len; i++)
                {
                    scaledArray[i] = array[i] * scalor[i];
                }
            }
            else
            {
                for (int i = 0; i < len; i++)
                {
                    if (scalor[i] != 0)
                    {
                        scaledArray[i] = array[i] / scalor[i];
                    }
                    else
                    {
                        // basic fix to prevent division by zero
                        scalor[i] = 0.00001;
                        scaledArray[i] = array[i] / scalor[i];
 
                    }
                }
            }
 
            return scaledArray;
        }
 
        public static double[,] Diag(this double[,] matrix, double[] diagVals)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            // the matrix has to be scare
            if (rows==cols)
            {
                double[,] diagMatrix = new double[rows, cols];
                int k = 0;
                for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++ )
                {
                    if(i==j)
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
            else
            {
                Console.WriteLine("Diag should be used on square matrix only.");
                return null;
            }
 
             
        }
    }
}