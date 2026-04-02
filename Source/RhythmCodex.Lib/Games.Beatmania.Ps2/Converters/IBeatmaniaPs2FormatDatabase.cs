using RhythmCodex.Games.Beatmania.Ps2.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

public interface IBeatmaniaPs2FormatDatabase
{
    BeatmaniaPs2FormatType? GetTypeByExeName(string name);
    BeatmaniaPs2FormatInfo? GetFormatByType(BeatmaniaPs2FormatType type);
}