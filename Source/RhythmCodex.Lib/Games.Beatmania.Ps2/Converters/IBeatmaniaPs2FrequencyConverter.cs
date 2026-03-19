namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

public interface IBeatmaniaPs2FrequencyConverter
{
    double Convert(int coarse, int fine);
}