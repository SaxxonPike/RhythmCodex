using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Beatmania.Models
{
    [Model]
    public class BeatmaniaPsxFolder
    {
        public IList<BeatmaniaPsxFile> Files { get; set; }
    }
}