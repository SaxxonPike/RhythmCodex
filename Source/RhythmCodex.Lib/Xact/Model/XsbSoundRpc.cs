using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model;

[Model]
public struct XsbSoundRpc
{
    public int[] Curves { get; set; }
    public byte[] ExtraData { get; set; }
}