using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Archs.Psx.Model;

[Model]
public class PsxBeatmaniaSongGroup
{
    public int Index { get; set; }
    public List<PsxBeatmaniaFile> Files { get; set; } = [];
}