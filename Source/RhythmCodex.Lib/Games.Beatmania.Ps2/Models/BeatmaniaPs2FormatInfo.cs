using System.Collections.Generic;

namespace RhythmCodex.Games.Beatmania.Ps2.Models;

public record struct BeatmaniaPs2FormatInfo
{
    public BeatmaniaPs2FormatType Type { get; init; }
    public required IReadOnlyList<BeatmaniaPs2FormatMetaTable> MetaTables { get; init; }
    public bool UseOldReaders { get; init; }
}