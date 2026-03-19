using System.Collections.Generic;

namespace RhythmCodex.Archs.Psx.Model;

public class PsxMgsSoundBankBlock
{
    public List<PsxMgsSoundBankBlockPatch> Patches { get; set; } = [];
}