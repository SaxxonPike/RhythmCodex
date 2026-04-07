using System;

namespace RhythmCodex.Games.Mgs.Models;

public record MgsSdSoundBankEntryWithData
{
    public int Index { get; init; }
    public required MgsSdSoundBankEntry Entry { get; init; }
    public ReadOnlyMemory<byte> Data { get; init; }
}