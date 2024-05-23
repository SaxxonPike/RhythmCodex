using RhythmCodex.Infrastructure;

namespace RhythmCodex.Vag.Models;

[Model]
public class SvagContainer
{
    public BigRational SampleRate { get; set; }
    public VagChunk? VagChunk { get; set; }
}