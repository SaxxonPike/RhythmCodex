using System.Numerics;

namespace RhythmCodex.Infrastructure;

public static class BigRationalParser
{
    public static BigRational? ParseString(string value)
    {
        if (value.Contains('/'))
        {
            var values = value.Split('/');
            return ParseString(values[0]) / ParseString(values[1]);
        }

        var numerator = BigInteger.Zero;
        var denominator = BigInteger.One;
        var isPastDecimalPoint = false;
        var canBeNegative = true;
        var canBeDecimalPoint = true;
        var isNegative = false;

        foreach (var c in value.Trim())
        {
            switch (c)
            {
                case >= '0' and <= '9':
                {
                    if (isPastDecimalPoint)
                        denominator *= 10;
                    numerator *= 10;
                    numerator += c - '0';
                    break;
                }
                case '.' when canBeDecimalPoint:
                    isPastDecimalPoint = true;
                    canBeDecimalPoint = false;
                    break;
                case '-' when canBeNegative:
                    isNegative = true;
                    break;
                default:
                    return null;
            }

            canBeNegative = false;
        }

        if (isNegative)
            numerator = -numerator;

        return new BigRational(numerator, denominator);
    }
}