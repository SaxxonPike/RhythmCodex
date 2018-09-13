using RhythmCodex.Infrastructure;

namespace RhythmCodex.Vag.Models
{
    [Model]
    public class VagChunk
    {
        public byte[] Data { get; set; }
        public int Channels { get; set; }
        public int Interleave { get; set; }
        public long? Length { get; set; }
    }
}