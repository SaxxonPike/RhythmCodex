using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ssq.Model
{
    [Model]
    public class SsqChunk
    {
        public short Parameter0 { get; init; }
        public short Parameter1 { get; init; }
        public byte[] Data { get; init; }
    }
}