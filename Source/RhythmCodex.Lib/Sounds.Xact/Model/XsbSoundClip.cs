using RhythmCodex.Infrastructure;

namespace RhythmCodex.Sounds.Xact.Model;

[Model]
public struct XsbSoundClip
{
    public byte Volume { get; set; }
    public int ClipOffset { get; set; }
    public short FilterFlags { get; set; }
    public short FilterFrequency { get; set; }
    public XsbSoundClipEvent[] Events { get; set; }
}