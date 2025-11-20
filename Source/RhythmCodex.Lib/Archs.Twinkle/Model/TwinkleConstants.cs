using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Converters;

namespace RhythmCodex.Archs.Twinkle.Model;

public static class TwinkleConstants
{
    // orig: 598248
    // test1: 598186
    public static BigRational BeatmaniaRate => new(598186, 10000);

    public const int ChunkSize = 0x1A00000;

    private static readonly Lazy<BigRational[]> VolumeTableLazy = new(() =>
    {
        // This algorithm is based on the one written into MAME:
        // https://github.com/mamedev/mame/blob/master/src/devices/sound/rf5c400.cpp

        var result = new BigRational[256];
        var max = BigRational.One;
        var step = Decibels.ToFactor(4.5d / 16d);

        for (var i = 0; i < 256; i++)
        {
            result[i] = max;
            max /= step;
        }

        return result;
    });
        
    public static BigRational[] VolumeTable => VolumeTableLazy.Value;
}