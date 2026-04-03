using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Models;

public record BeatmaniaPs2ChartSet
{
    public int SongId { get; set; }
    public string? Name { get; set; }
    public Dictionary<int, (int BgmId, int KeysoundId)> ChartMaps { get; set; } = [];
    public Dictionary<int, Chart> Charts { get; set; } = [];
    public Dictionary<int, Sound> Bgms { get; set; } = [];
    public Dictionary<int, List<Sound>> Keysounds { get; set; } = [];
}