using System;

namespace RhythmCodex.Archs.Psx.Model;

public sealed record PsxBeatmaniaFile
{
    public int Index { get; set; }
    public int Group { get; set; }
    public int GroupIndex { get; set; }
    public ReadOnlyMemory<byte> Data { get; set; }
    public PsxBeatmaniaFileType Type { get; set; }
}