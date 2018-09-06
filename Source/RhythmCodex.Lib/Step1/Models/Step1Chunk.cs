using RhythmCodex.Infrastructure;

namespace RhythmCodex.Step1.Models
{
    [Model]
    public class Step1Chunk
    {
        public int Metadata { get; set; }
        public byte[] Data { get; set; }
    }
}