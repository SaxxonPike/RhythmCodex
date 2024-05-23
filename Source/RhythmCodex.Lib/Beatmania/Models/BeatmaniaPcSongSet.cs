using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Beatmania.Models;

[Model]
public class BeatmaniaPcSongSet
{
    public IList<BeatmaniaPcAudioEntry> Sounds { get; set; } = [];
    public IList<BeatmaniaPc1Chart> Charts { get; set; } = [];
}