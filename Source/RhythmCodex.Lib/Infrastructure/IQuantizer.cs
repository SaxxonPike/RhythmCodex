using System.Collections.Generic;
using System.Numerics;

namespace RhythmCodex.Infrastructure;

public interface IQuantizer
{
    int GetQuantization(IEnumerable<BigRational> rationals);
    int GetQuantization(IEnumerable<BigRational> rationals, BigInteger minimum, BigInteger maximum);
}