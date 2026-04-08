using System.Collections.Generic;
using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Models;

[Model]
public record BeatmaniaPs2Keysound
{
    public int Index { get; set; }
    public int SampleNumber { get; set; }
    public int Reserved0 { get; set; }
    public int Channel { get; set; }
    public int PanningLeft { get; set; }
    public int PanningRight { get; set; }
    public int PanningScale { get; set; } = 127;
    public int Volume { get; set; }
    public int VolumeScale { get; set; } = 127;
    public int SampleType { get; set; }
    public float FrequencyLeft { get; set; }
    public float FrequencyRight { get; set; }
    public int OffsetLeft { get; set; }
    public int OffsetRight { get; set; }
    public int Reserved1 { get; set; }
    public int Reserved2 { get; set; }
    public List<VagChunk> Data { get; set; } = [];
}