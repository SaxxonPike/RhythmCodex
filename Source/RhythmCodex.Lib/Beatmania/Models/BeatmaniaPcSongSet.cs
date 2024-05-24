using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Beatmania.Models;

[Model]
public class BeatmaniaPcSongSet
{
    public List<BeatmaniaPcAudioEntry> Sounds { get; set; } = [];
    public List<BeatmaniaPc1Chart> Charts { get; set; } = [];
}