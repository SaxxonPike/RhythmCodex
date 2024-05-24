using System;
using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Beatmania.Converters;

public class BeatmaniaPs2SampleRateFinder : IBeatmaniaPs2SampleRateFinder
{
    private static readonly IReadOnlyDictionary<int, int> KnownFrequencies = new Dictionary<int, int>
    {
        {0x3E44, 0xAC44},
        {0x3F44, 0xA298},
        {0x406B, 0x9C40},
        {0x4102, 0x8CA0},
        {0x413F, 0x9088},
        {0x4144, 0x9088},
        {0x417A, 0x93BA},
        {0x4203, 0x8CA0},
        {0x4244, 0x88BA},
        {0x4346, 0x8340},
        {0x4444, 0x7D00},
        {0x447D, 0x7D00},
        {0x456E, 0x7530},
        {0x4655, 0x6D60},
        {0x4800, 0x5DC0},
        {0x4A3F, 0x5622},
        {0x4A44, 0x5622},
        {0x4D02, 0x2B11},
        {0x507D, 0x3E80}
    };


    public BigRational GetRate(int encodedRate)
    {
        if (KnownFrequencies.TryGetValue(encodedRate, out var rate))
            return rate;
            
        // TODO: actually figure out this algo for once
        throw new NotImplementedException();
    }
}