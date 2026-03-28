using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Games.Beatmania.Psx.Models;

[Model]
public record PsxBeatmaniaSongGroup
{
    public int Index { get; set; }
    public List<PsxBeatmaniaFile> Files { get; set; } = [];
}