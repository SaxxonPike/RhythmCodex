using System;

namespace RhythmCodex.Archs.Psx.Model;

public sealed class PsxBeatmaniaKeysound
{
    public int Index { get; set; }
    public required PsxMgsSoundBankEntry Info { get; set; }
    public ReadOnlyMemory<byte> Data { get; set; }
}