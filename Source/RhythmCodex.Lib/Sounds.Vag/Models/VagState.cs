using RhythmCodex.Infrastructure;

namespace RhythmCodex.Sounds.Vag.Models;

[Model]
public class VagState
{
    public int Prev0 { get; set; }
    public int Prev1 { get; set; }
    public bool Enabled { get; set; } = true;
}