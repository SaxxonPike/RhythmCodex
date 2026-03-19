using System;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Psx.Converters;

[Service]
public sealed class PsxMgsUnitConverter : IPsxMgsUnitConverter
{
    private static readonly Lazy<byte[]> PanTable1 = new(() =>
    [
        0, 2, 4, 7, 10, 13, 16, 20, 24, 28, 32, 36, 40, 45,
        50, 55, 60, 65, 70, 75, 80, 84, 88, 92, 96, 100, 104, 107,
        110, 112, 114, 116, 118, 120, 122, 123, 124, 125, 126, 127, 127
    ]);

    private static readonly Lazy<byte[]> PanTable2 = new(() =>
    [
        0, 2, 4, 6, 8, 10, 14, 18, 22, 28, 34, 40, 46,
        52, 58, 64, 70, 76, 82, 88, 94, 100, 106, 112, 118, 124,
        130, 136, 142, 148, 154, 160, 166, 172, 178, 183, 188, 193, 198,
        203, 208, 213, 217, 221, 224, 227, 230, 233, 236, 238, 240, 242,
        244, 246, 248, 249, 250, 251, 252, 253, 254, 254, 255, 255, 255
    ]);

    private static readonly Lazy<ushort[]> FreqTable = new(() =>
    [
        0x010B, 0x011B, 0x012C, 0x013E, 0x0151, 0x0165, 0x017A, 0x0191,
        0x01A9, 0x01C2, 0x01DD, 0x01F9, 0x0217, 0x0237, 0x0259, 0x027D,
        0x02A3, 0x02CB, 0x02F5, 0x0322, 0x0352, 0x0385, 0x03BA, 0x03F3,
        0x042F, 0x046F, 0x04B2, 0x04FA, 0x0546, 0x0596, 0x05EB, 0x0645,
        0x06A5, 0x070A, 0x0775, 0x07E6, 0x085F, 0x08DE, 0x0965, 0x09F4,
        0x0A8C, 0x0B2C, 0x0BD6, 0x0C8B, 0x0D4A, 0x0E14, 0x0EEA, 0x0FCD,
        0x10BE, 0x11BD, 0x12CB, 0x13E9, 0x1518, 0x1659, 0x17AD, 0x1916,
        0x1A94, 0x1C28, 0x1DD5, 0x1F9B, 0x217C, 0x237A, 0x2596, 0x27D2,
        0x2A30, 0x2CB2, 0x2F5A, 0x322C, 0x3528, 0x3850, 0x3BAC, 0x3F36,
        0x0021, 0x0023, 0x0026, 0x0028, 0x002A, 0x002D, 0x002F, 0x0032,
        0x0035, 0x0038, 0x003C, 0x003F, 0x0042, 0x0046, 0x004B, 0x004F,
        0x0054, 0x0059, 0x005E, 0x0064, 0x006A, 0x0070, 0x0077, 0x007E,
        0x0085, 0x008D, 0x0096, 0x009F, 0x00A8, 0x00B2, 0x00BD, 0x00C8,
        0x00D4, 0x00E1, 0x00EE, 0x00FC
    ]);

    private static readonly Lazy<byte[]> RdmTable = new(() =>
    [
        159, 60, 178, 82, 175, 69, 199, 137,
        16, 127, 224, 157, 220, 31, 97, 22,
        57, 201, 156, 235, 87, 8, 102, 248,
        90, 36, 191, 14, 62, 21, 75, 219,
        171, 245, 49, 12, 67, 2, 85, 222,
        65, 218, 189, 174, 25, 176, 72, 87,
        186, 163, 54, 11, 249, 223, 23, 168,
        4, 12, 224, 145, 24, 93, 221, 211,
        40, 138, 242, 17, 89, 111, 6, 10,
        52, 42, 121, 172, 94, 167, 131, 198,
        57, 193, 180, 58, 63, 254, 79, 239,
        31, 0, 48, 153, 76, 40, 131, 237,
        138, 47, 44, 102, 63, 214, 108, 183,
        73, 34, 188, 101, 250, 207, 2, 177,
        70, 240, 154, 215, 226, 15, 17, 197,
        116, 246, 122, 44, 143, 251, 25, 106,
        229
    ]);

    private static readonly Lazy<byte[]> VibTable = new(() =>
    [
        0, 32, 56, 80, 104, 128, 144, 160,
        176, 192, 208, 224, 232, 240, 240, 248,
        255, 248, 244, 240, 232, 224, 208, 192,
        176, 160, 144, 128, 104, 80, 56, 32
    ]);

    public (float Left, float Right) ConvertGain(int volume, int panning)
    {
        var pan = panning >> 8;

        if (pan > 40)
            pan = 40;

        var panTable = PanTable1.Value;
        var rightVol = volume * panTable[pan];
        var leftVol = volume * panTable[40 - pan];

        return (leftVol / 32767f, rightVol / 32767f);
    }

    public float ConvertFrequency(int value, int microTune, int macroTune)
    {
        var tune = value + unchecked((sbyte)microTune);
        var note = ((tune >> 8) + unchecked((byte)macroTune)) & 0x7F;
        var freq = FreqTable.Value[note + 1] - FreqTable.Value[note];

        if ((freq & 0x8000) != 0)
            freq = 0xC9;

        return ((freq * tune) >> 8) + (freq >> 8) * tune + FreqTable.Value[note];
    }

    public int GetRandom(int sequence)
    {
        var table = RdmTable.Value;
        return table[sequence % table.Length];
    }

    public int GetVibrato(int sequence)
    {
        var table = VibTable.Value;
        return table[sequence % table.Length];
    }
}