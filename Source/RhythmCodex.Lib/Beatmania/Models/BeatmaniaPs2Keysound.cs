using System.Collections.Generic;
using RhythmCodex.Infrastructure;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Beatmania.Models;

[Model]
public class BeatmaniaPs2Keysound
{
    public int SampleNumber { get; set; }
    public int Reserved0 { get; set; }
    public int Channel { get; set; }
    public int Volume { get; set; }
    public int Panning { get; set; }
    public int SampleType { get; set; }
    public int FrequencyLeft { get; set; }
    public int FrequencyRight { get; set; }
    public int OffsetLeft { get; set; }
    public int OffsetRight { get; set; }
    public int PseudoLeft { get; set; }
    public int PseudoRight { get; set; }
    public List<VagChunk> Data { get; set; } = [];
}