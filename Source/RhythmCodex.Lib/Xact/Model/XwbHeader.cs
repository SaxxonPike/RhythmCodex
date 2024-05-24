using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model;

[Model]
public struct XwbHeader
{
    public int Signature { get; set; }
    public int Version { get; set; }
    public int HeaderVersion { get; set; }
    public XwbRegion[] Segments { get; set; }
}