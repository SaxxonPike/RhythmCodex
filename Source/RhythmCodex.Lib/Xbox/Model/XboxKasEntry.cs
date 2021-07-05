using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xbox.Model
{
    [Model]
    public class XboxKasEntry
    {
        public int Block { get; set; }
        public int Offset { get; set; }
        public byte[] Data { get; set; }
    }
}