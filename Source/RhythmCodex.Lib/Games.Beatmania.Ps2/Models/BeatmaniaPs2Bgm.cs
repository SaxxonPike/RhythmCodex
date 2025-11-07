using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Models;

[Model]
public class BeatmaniaPs2Bgm
{
    public int Volume { get; set; }
    public int Channels { get; set; }
    public int Rate { get; set; }
    public VagChunk? Data { get; set; }
}