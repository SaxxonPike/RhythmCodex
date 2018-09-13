namespace RhythmCodex.Xa.Models
{
    public class XaChunk
    {
        public byte[] Data { get; set; }
        public int Channels { get; set; }
        public int Interleave { get; set; }
    }
}