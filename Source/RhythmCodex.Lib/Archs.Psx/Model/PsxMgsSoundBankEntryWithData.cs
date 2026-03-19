using System;

namespace RhythmCodex.Archs.Psx.Model;

public class PsxMgsSoundBankEntryWithData
{
    public int Index { get; set; }
    public required PsxMgsSoundBankEntry Entry { get; set; }
    public ReadOnlyMemory<byte> Data { get; set; }
}