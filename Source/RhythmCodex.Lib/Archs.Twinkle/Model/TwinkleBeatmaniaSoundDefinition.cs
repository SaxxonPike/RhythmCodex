using RhythmCodex.Infrastructure;

namespace RhythmCodex.Archs.Twinkle.Model;

[Model]
public class TwinkleBeatmaniaSoundDefinition
{
    public int Channel { get; set; }
    public int Flags01 { get; set; }
    public int Frequency { get; set; }
    public int Volume { get; set; }
    public int Panning { get; set; }
    public int SampleStart { get; set; }
    public int SampleEnd { get; set; }
    public int Value0C { get; set; }
    public int Flags0E { get; set; }
    public int Flags0F { get; set; }
    public int SizeInBlocks { get; set; }
}