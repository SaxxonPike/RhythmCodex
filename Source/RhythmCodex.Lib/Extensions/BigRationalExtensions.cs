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
            var output = BigInteger.One;
            var values = rationals.ToArray();
            
            while (values.Any(v => !v.Denominator.IsOne))
            {
                var multiplier = values.First(v => !v.Denominator.IsOne).Denominator;
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] *= multiplier;
                    output *= multiplier;
                }
            }

            return output;
        }
    }
}
