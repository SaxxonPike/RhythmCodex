using System;

namespace RhythmCodex.Sounds.Converters;

public static class Decibels
{
    public static double ToFactor(double gainDb) => 
        Math.Pow(10.0f, gainDb / 20.0f);
}