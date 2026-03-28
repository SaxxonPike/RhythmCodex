using System;

namespace RhythmCodex.Games.Beatmania.Psx.Models;

public record PsxBeatmaniaSysFile
{
    public int Index { get; set; }
    public ReadOnlyMemory<byte> Data { get; set; }
    public PsxBeatmaniaFileType Type { get; set; }
}