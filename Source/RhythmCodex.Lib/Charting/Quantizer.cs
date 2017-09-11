using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Charting
{
    public class Quantizer : IQuantizer
    {
        public int GetQuantization(IEnumerable<BigRational> rationals)
        {
            return GetQuantization(rationals, BigInteger.One, int.MaxValue);
        }
        
        public int GetQuantization(IEnumerable<BigRational> rationals, BigInteger minimum, BigInteger maximum)
        {
            var data = rationals.AsList();
            var multiplier = GetQuantizationInternal(data, minimum, maximum);
                
            if (multiplier > maximum)
            {
                // Requantize. This results in a loss of precision.
                multiplier =
                    GetQuantizationInternal(data.Select(n => new BigRational((n * maximum).Numerator, maximum)),
                        minimum, maximum);
            }

            return (int) multiplier;
        }
        
        private static BigInteger GetQuantizationInternal(IEnumerable<BigRational> rationals, BigInteger start, BigInteger threshold)
        {
            var values = rationals.Select(r => r.GetFractionPart() * start).ToArray();
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
}