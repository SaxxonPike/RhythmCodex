using System;

namespace RhythmCodex.Games.Mgs.Models;

public record MgsSdSoundBankBlockPatch
{
    public int Address { get; init; }
    public int Length { get; init; }
    public ReadOnlyMemory<byte> Data { get; init; }
}