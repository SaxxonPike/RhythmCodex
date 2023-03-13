using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;

namespace RhythmCodex.Infrastructure;

/// <summary>
///     Quantizes BigRational values for efficient representation within a metric time system.
/// </summary>
[Service]
public class Quantizer : IQuantizer
{
    public int GetQuantization(IEnumerable<BigRational> rationals)
    {
        return GetQuantization(rationals, BigInteger.One, int.MaxValue);
    }

    public int GetQuantization(IEnumerable<BigRational> rationals, BigInteger minimum, BigInteger maximum)
    {
        if (maximum < minimum)
            maximum = minimum;
            
        var data = rationals.AsList();
        var multiplier = GetQuantizationInternal(data, minimum, maximum);

        if (multiplier > maximum)
            multiplier =
                GetQuantizationInternal(data.Select(n => new BigRational((n * maximum).GetWholePart(), maximum)),
                    minimum, maximum);

        return (int) multiplier;
    }

    private static BigInteger GetQuantizationInternal(IEnumerable<BigRational> rationals, BigInteger start,
        BigInteger threshold)
    {
        var values = rationals.Select(r => r.GetFractionPart() * new BigRational(start)).ToArray();
        if (values.All(v => v.Numerator.IsZero))
            return start;

        var output = start;
        while (values.Any(v => !v.Denominator.IsOne))
        {
            var multiplier = values.First(v => !v.Denominator.IsOne).Denominator;
            for (var i = 0; i < values.Length; i++)
                values[i] *= multiplier;

            output *= multiplier;

            if (output > threshold)
                return output;
        }

        return output;
    }
}