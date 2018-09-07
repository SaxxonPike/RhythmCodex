using System.Numerics;

namespace RhythmCodex.Infrastructure
{
    public static class BigRationalParser
    {
        public static BigRational? ParseString(string value)
        {
            BigInteger numerator = 0;
            BigInteger denominator = 1;
            var isPastDecimalPoint = false;
            var canBeNegative = true;
            var canBeDecimalPoint = true;
            var isNegative = false;

            foreach (var c in value.Trim())
            {
                if (c >= '0' && c <= '9')
                {
                    if (isPastDecimalPoint)
                        denominator *= 10;
                    numerator *= 10;
                    numerator += c - '0';
                }
                else if (c == '.' && canBeDecimalPoint)
                {
                    isPastDecimalPoint = true;
                    canBeDecimalPoint = false;
                }
                else if (c == '-' && canBeNegative)
                {
                    isNegative = true;
                }
                else
                {
                    return null;
                }
                canBeNegative = false;
            }

            if (isNegative)
                numerator = -numerator;

            return new BigRational(numerator, denominator);
        }
    }
}