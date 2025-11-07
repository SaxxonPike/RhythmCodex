using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Games.Beatmania.Pc.Models;

[Model]
public class BeatmaniaPcSongSet
{
    public List<BeatmaniaPcAudioEntry> Sounds { get; set; } = [];
    public List<BeatmaniaPc1Chart> Charts { get; set; } = [];
}