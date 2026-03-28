using System;

namespace RhythmCodex.Archs.Psx.Model;

public record PsxMgsSound
{
    public int Index { get; set; }
    public required PsxMgsSoundScript Script { get; set; }
    public ReadOnlyMemory<byte> Data { get; set; }
}