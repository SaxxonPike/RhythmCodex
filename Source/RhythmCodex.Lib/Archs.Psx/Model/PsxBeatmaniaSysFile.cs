using System;

namespace RhythmCodex.Archs.Psx.Model;

public record PsxBeatmaniaSysFile
{
    public int Index { get; set; }
    public ReadOnlyMemory<byte> Data { get; set; }
    public PsxBeatmaniaFileType Type { get; set; }
}