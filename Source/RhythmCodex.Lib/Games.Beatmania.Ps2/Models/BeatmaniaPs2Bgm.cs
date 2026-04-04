using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Models;

[Model]
public record BeatmaniaPs2Bgm
{
    public int Index { get; set; }
    public int Volume { get; set; }
    public int VolumeScale { get; set; } = 127;
    public int Channels { get; set; }
    public int Rate { get; set; }
    public int Skip { get; set; }
    public VagChunk? Data { get; set; }
}