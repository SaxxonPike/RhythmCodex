using RhythmCodex.Infrastructure;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Beatmania.Ps2.Models;

[Model]
public class BeatmaniaPs2Bgm
{
    public int Volume { get; set; }
    public int Channels { get; set; }
    public int Rate { get; set; }
    public VagChunk? Data { get; set; }
}