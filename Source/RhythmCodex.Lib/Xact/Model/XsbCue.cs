using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model;

[Model]
public struct XsbCue
{
    public string Name { get; set; }
    public byte Flags { get; set; }
    public int Offset { get; set; }
    public XsbSound Sound { get; set; }
}