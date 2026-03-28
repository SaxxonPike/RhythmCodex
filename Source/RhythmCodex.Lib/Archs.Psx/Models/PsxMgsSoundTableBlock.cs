using System;

namespace RhythmCodex.Archs.Psx.Model;

public record PsxMgsSoundTableBlock
{
    public ReadOnlyMemory<byte> Header { get; set; }
    public ReadOnlyMemory<byte> Table { get; set; }
    public ReadOnlyMemory<byte> Scripts { get; set; }
}