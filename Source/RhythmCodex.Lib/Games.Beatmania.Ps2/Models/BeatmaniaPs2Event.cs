using RhythmCodex.Infrastructure;

namespace RhythmCodex.Games.Beatmania.Ps2.Models;

[Model]
public class BeatmaniaPs2Event
{
    public int LinearOffset { get; set; }
    public byte Type { get; set; }
    public byte Parameter0 { get; set; }
    public byte Parameter1 { get; set; }
    public short Value { get; set; }
}