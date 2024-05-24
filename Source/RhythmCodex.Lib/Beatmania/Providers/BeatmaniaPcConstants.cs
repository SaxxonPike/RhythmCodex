using System;
using System.Linq;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Beatmania.Providers;

public static class BeatmaniaPcConstants
{
    private static readonly Lazy<ReadOnlyMemory<BigRational>> VolumeTableLazy = new(() =>
        Enumerable
            .Range(0, 256)
            .Select(i => new BigRational(Math.Pow(10.0f, -36.0f * i / 64f / 20.0f)))
            .ToArray()
    );
        
    public static readonly ReadOnlyMemory<BigRational> VolumeTable = VolumeTableLazy.Value;
}