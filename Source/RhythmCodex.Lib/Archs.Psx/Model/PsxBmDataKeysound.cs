using System;

namespace RhythmCodex.Archs.Psx.Model;

public class PsxBmDataKeysound
{
    public int Index { get; set; }
    public required PsxBmDataKeysoundInfo Info { get; set; }
    public ReadOnlyMemory<byte> Data { get; set; }
}