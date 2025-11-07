using RhythmCodex.Infrastructure;

namespace RhythmCodex.Games.Ddr.Models;

[Model]
public class MetadataDecoratorFileExtensions
{
    public string Audio { get; set; } = "wav";
    public string Graphics { get; set; } = "png";
    public string Video { get; set; } = "mkv";
}