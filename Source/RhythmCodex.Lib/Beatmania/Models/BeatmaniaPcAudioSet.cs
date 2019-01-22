using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Beatmania.Models
{
    [Model]
    public class BeatmaniaPcAudioSet
    {
        public string Name { get; set; }
        public IEnumerable<BeatmaniaPcAudioEntry> Entries { get; set; }
    }
}