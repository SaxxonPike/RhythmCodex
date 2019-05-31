using System;
using System.Linq;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Twinkle.Model
{
    public static class TwinkleConstants
    {
        public static BigRational BeatmaniaRate => new BigRational(598, 10);
        
        public static readonly BigRational[] VolumeTable =
            Enumerable
                .Range(0, 256)
                .Select(i => BigRational.One / new BigRational(Math.Pow(10.0f, 4.5f / 16f / 20.0f)))
                .ToArray();
    }
}