using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model;

[Model]
public class XwbSound
{
    public required byte[] Data { get; set; }
    public required string Name { get; set; }
    public required XwbEntry Info { get; set; }
}