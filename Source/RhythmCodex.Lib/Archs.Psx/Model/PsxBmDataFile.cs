using System;

namespace RhythmCodex.Archs.Psx.Model;

public record PsxBmDataFile
{
    public int Index { get; set; }
    public int Group { get; set; }
    public int GroupIndex { get; set; }
    public ReadOnlyMemory<byte> Data { get; set; }
    public PsxBeatmaniaFileType Type { get; set; }
}