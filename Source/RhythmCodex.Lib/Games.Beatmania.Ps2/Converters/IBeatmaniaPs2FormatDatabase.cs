using RhythmCodex.Games.Beatmania.Ps2.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

public interface IBeatmaniaPs2FormatDatabase
{
    BeatmaniaPs2FormatInfo? GetForExecutableName(string name);
    BeatmaniaPs2FormatInfo? GetForType(BeatmaniaPs2FormatType type);
}