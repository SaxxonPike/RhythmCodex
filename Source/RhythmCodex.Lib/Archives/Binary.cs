using RhythmCodex.Attributes;

namespace RhythmCodex.Archives
{
    public class Binary : Metadata, IBinary
    {
        public byte[] Data { get; set; }
    }
}