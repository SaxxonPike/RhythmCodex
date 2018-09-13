using RhythmCodex.Infrastructure;

namespace RhythmCodex.Vag.Models
{
    [Model]
    public class SvagContainer
    {
        public int SampleRate { get; set; }
        public VagChunk VagChunk { get; set; }
    }
}