using System;

namespace RhythmCodex.Archs.Psx.Model;

public sealed class PsxMgsSound
{
    public int Index { get; set; }
    public required PsxMgsSoundScript Script { get; set; }
    public ReadOnlyMemory<byte> Data { get; set; }
}