using RhythmCodex.Infrastructure;

namespace RhythmCodex.Games.Beatmania.Ps2.Models;

[Model]
public class BeatmaniaPs2Event
{
    public int LinearOffset { get; set; }
    public BeatmaniaPs2EventType Type { get; set; }
    public int Parameter { get; set; }
    public int Value { get; set; }
}