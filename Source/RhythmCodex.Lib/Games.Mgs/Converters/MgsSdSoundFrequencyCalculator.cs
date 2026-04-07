using System;
using RhythmCodex.IoC;

namespace RhythmCodex.Games.Mgs.Converters;

/// <inheritdoc />
[Service]
public sealed class MgsSdSoundFrequencyCalculator : IMgsSdSoundFrequencyCalculator
{
    /// <summary>
    /// Frequency table used by the Metal Gear Solid sound system.
    /// </summary>
    private static readonly Lazy<ReadOnlyMemory<int>> FreqTbl = new(() => (int[])
    [
        0x010B, 0x011B, 0x012C, 0x013E, 0x0151, 0x0165, 0x017A, 0x0191, 0x01A9, 0x01C2, 0x01DD, 0x01F9,
        0x0217, 0x0237, 0x0259, 0x027D, 0x02A3, 0x02CB, 0x02F5, 0x0322, 0x0352, 0x0385, 0x03BA, 0x03F3,
        0x042F, 0x046F, 0x04B2, 0x04FA, 0x0546, 0x0596, 0x05EB, 0x0645, 0x06A5, 0x070A, 0x0775, 0x07E6,
        0x085F, 0x08DE, 0x0965, 0x09F4, 0x0A8C, 0x0B2C, 0x0BD6, 0x0C8B, 0x0D4A, 0x0E14, 0x0EEA, 0x0FCD,
        0x10BE, 0x11BD, 0x12CB, 0x13E9, 0x1518, 0x1659, 0x17AD, 0x1916, 0x1A94, 0x1C28, 0x1DD5, 0x1F9B,
        0x217C, 0x237A, 0x2596, 0x27D2, 0x2A30, 0x2CB2, 0x2F5A, 0x322C, 0x3528, 0x3850, 0x3BAC, 0x3F36,
        0x0021, 0x0023, 0x0026, 0x0028, 0x002A, 0x002D, 0x002F, 0x0032, 0x0035, 0x0038, 0x003C, 0x003F,
        0x0042, 0x0046, 0x004B, 0x004F, 0x0054, 0x0059, 0x005E, 0x0064, 0x006A, 0x0070, 0x0077, 0x007E,
        0x0085, 0x008D, 0x0096, 0x009F, 0x00A8, 0x00B2, 0x00BD, 0x00C8, 0x00D4, 0x00E1, 0x00EE, 0x00FC
    ]);

    /// <inheritdoc />
    public float Calculate(int note, int macro, int micro)
    {
        //
        // Determine the coarse and fine-tune components.
        //

        var noteTune = ((note + macro) << 8) + (micro << 1);
        var fineTune = unchecked((byte)noteTune);
        var coarseTune = (noteTune >> 8) & 0x7F;

        //
        // Use linear interpolation to convert the fine-tune value to whole cycles.
        //

        var coarseFreqTable = FreqTbl.Value.Span;
        float coarseTuneCycles = coarseFreqTable[coarseTune];
        var toneScale = coarseFreqTable[coarseTune + 1] - coarseTuneCycles;

        if (toneScale < 0)
            toneScale = 0xC9;

        var fineTuneCycles = fineTune / 128f * toneScale;
        var totalCycles = Math.Clamp(MathF.Round(coarseTuneCycles + fineTuneCycles), 0, 16384f);

        var result = 44100f * totalCycles / 0x1000;
        return result;
    }
}