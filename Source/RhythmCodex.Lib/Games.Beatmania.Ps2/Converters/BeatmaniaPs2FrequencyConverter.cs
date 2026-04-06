using System;
using RhythmCodex.IoC;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

/// <inheritdoc />
[Service]
public sealed class BeatmaniaPs2FrequencyConverter : IBeatmaniaPs2FrequencyConverter
{
    /// <inheritdoc />
    public double Convert(int coarse, int fine)
    {
        //
        // Source: "trust me bro."
        // It took forever to discover this is treated as a two-part frequency calculation:
        // a coarse-tuning (unsure the range) and fine-tuning (8-bit signed). An octave is represented
        // as 12 coarse steps (like the chromatic scale on a piano). The fine-tuning is then an
        // adjustment on top of that, which has a range of about 3 coarse steps over 127 values.
        // The final constant was brute forced and comes out to a value close to 1/255*6 if I
        // adjust to {coarse=0x3E, fine=0x44} -> ~44100. The old lookup table was empirically built:
        // compare exact frequencies from the old format to the new format for songs that were revived.
        // It turned out to be acceptable, but not accurate.
        //

        var coarseVal = unchecked((byte)coarse);
        var fineVal = unchecked((sbyte)fine);

        const double coarseTuneScale = 12d;
        const double fineTuneScale = 1531.155d;
        const double maxFrequency = 1536000d;

        return maxFrequency / Math.Pow(2, coarseVal / coarseTuneScale - fineVal / fineTuneScale);
    }
}