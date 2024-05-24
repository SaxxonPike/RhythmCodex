using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xbox.Model;

[Model]
public class XboxHbnEntry
{
    public string? Name { get; set; }
    public byte[]? Data { get; set; }
}