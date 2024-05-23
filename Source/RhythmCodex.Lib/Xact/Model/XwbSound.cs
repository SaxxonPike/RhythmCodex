using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model;

[Model]
public class XwbSound
{
    public byte[] Data { get; set; }
    public string Name { get; set; }
    public XwbEntry Info { get; set; }
}