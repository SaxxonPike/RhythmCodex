using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Numerics;

namespace RhythmCodex.Extensions
{
    public static class BigRationalExtensions
    {
        public static BigInteger GetWholeMultiplier(this IEnumerable<BigRational> rationals)
        {
            var values = rationals.Select(r => r.GetFractionPart()).ToArray();
            if (values.All(v => v.Numerator.IsZero))
                return BigInteger.One;

            var output = BigInteger.One;
            while (values.Any(v => !v.Denominator.IsOne))
            {
                var multiplier = values.First(v => !v.Denominator.IsOne).Denominator;
                for (var i = 0; i < values.Length; i++)
                    values[i] *= multiplier;

                output *= multiplier;
            }

            return output;
        }
    }
}
