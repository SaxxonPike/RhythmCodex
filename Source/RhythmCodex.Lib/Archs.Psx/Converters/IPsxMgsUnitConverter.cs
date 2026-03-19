namespace RhythmCodex.Archs.Psx.Converters;

public interface IPsxMgsUnitConverter
{
    (float Left, float Right) ConvertGain(int volume, int panning);
    float ConvertFrequency(int value, int microTune, int macroTune);
    int GetRandom(int sequence);
    int GetVibrato(int sequence);
}