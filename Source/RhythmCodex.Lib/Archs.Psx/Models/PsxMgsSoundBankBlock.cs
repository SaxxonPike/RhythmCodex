using System.Collections.Generic;

namespace RhythmCodex.Archs.Psx.Model;

public record PsxMgsSoundBankBlock
{
    public List<PsxMgsSoundBankBlockPatch> Patches { get; set; } = [];
}