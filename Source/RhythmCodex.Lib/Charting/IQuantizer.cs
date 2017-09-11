using System.Collections.Generic;
using System.Numerics;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Charting
{
    public interface IQuantizer
    {
        int GetQuantization(IEnumerable<BigRational> rationals);
        int GetQuantization(IEnumerable<BigRational> rationals, BigInteger minimum, BigInteger maximum);
    }
}