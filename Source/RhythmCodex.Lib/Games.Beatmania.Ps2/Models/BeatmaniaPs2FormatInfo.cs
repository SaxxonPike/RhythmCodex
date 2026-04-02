using System.Collections.Generic;

namespace RhythmCodex.Games.Beatmania.Ps2.Models;

public struct BeatmaniaPs2FormatInfo
{
    public required IReadOnlyList<BeatmaniaPs2FormatMetaTable> MetaTables { get; init; }
}