using System.Linq;

namespace RhythmCodex.Ssq.Model
{
    public class Chunk : IChunk
    {
        public int Parameter0 { get; set; }
        public int Parameter1 { get; set; }
        public byte[] Data { get; set; }
    }
}
