using RhythmCodex.Infrastructure;

namespace RhythmCodex.Archs.Xbox.Model;

[Model]
public class XboxHbnDefinition
{
    public string? Name { get; set; }
    public int Offset { get; set; }
    public int Length { get; set; }
}