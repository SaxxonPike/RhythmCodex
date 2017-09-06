using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ssq.Model
{
    [Model]
    public class Chunk : IChunk
    {
        public short Parameter0 { get; set; }
        public short Parameter1 { get; set; }
        public byte[] Data { get; set; }
    }
}
