using System.Collections.Generic;

namespace RhythmCodex.Games.Mgs.Models;

public record MgsSdSoundBankBlock
{
    public List<MgsSdSoundBankBlockPatch> Patches { get; init; } = [];
}