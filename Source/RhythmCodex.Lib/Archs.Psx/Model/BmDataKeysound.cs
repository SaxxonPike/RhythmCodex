using System;

namespace RhythmCodex.Archs.Psx.Model;

public class BmDataKeysound
{
    public int Index { get; set; }
    public required BmDataKeysoundInfo Info { get; set; }
    public ReadOnlyMemory<byte> Data { get; set; }
}