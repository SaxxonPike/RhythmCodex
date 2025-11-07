using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Beatmania.Psx.Models;

[Model]
public class BeatmaniaPsxFolder
{
    public List<BeatmaniaPsxFile> Files { get; set; } = [];
}