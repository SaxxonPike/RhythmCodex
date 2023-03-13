using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Twinkle.Model;

public static class TwinkleConstants
{
    // orig: 598248
    // test1: 598186
    public static BigRational BeatmaniaRate => new(598186, 10000);

    private static readonly Lazy<BigRational[]> VolumeTableLazy = new(() =>
    {
        var result = new BigRational[256];
        var max = BigRational.One;
        for (var i = 0; i < 256; i++)
        {
            result[i] = max;
            max /= new BigRational(Math.Pow(10.0f, 4.5f / 16f / 20.0f));
        }

        return result;
    });
        
    public static BigRational[] VolumeTable => VolumeTableLazy.Value;
}