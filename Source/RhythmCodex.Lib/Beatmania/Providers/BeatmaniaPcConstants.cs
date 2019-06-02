using System;
using System.Linq;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Beatmania.Providers
{
    public static class BeatmaniaPcConstants
    {
        private static readonly Lazy<BigRational[]> VolumeTableLazy = new Lazy<BigRational[]>(() =>
            Enumerable
                .Range(0, 256)
                .Select(i => new BigRational(Math.Pow(10.0f, -36.0f * i / 64f / 20.0f)))
                .ToArray()
        );
        
        public static readonly BigRational[] VolumeTable = VolumeTableLazy.Value;
    }
}