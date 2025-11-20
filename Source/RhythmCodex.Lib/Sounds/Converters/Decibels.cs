using System;

namespace RhythmCodex.Sounds.Converters;

/// <summary>
/// Contains functions that handle conversion of dB units.
/// </summary>
public static class Decibels
{
    /// <summary>
    /// Converts a decibel value into a multiplier for audio gain.
    /// </summary>
    public static double ToFactor(double gainDb) => 
        Math.Pow(10.0f, gainDb / 20.0f);
}