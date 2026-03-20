using System;

namespace RhythmCodex.Archs.Psx.Model;

public record PsxMgsSoundBankBlockPatch
{
    public int Address { get; set; }
    public int Length { get; set; }
    public ReadOnlyMemory<byte> Data { get; set; }
}