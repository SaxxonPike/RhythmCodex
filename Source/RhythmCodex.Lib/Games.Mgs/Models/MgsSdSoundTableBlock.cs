using System;

namespace RhythmCodex.Games.Mgs.Models;

public record MgsSdSoundTableBlock
{
    public ReadOnlyMemory<byte> Header { get; init; }
    public ReadOnlyMemory<byte> Table { get; init; }
    public ReadOnlyMemory<byte> Scripts { get; init; }
}