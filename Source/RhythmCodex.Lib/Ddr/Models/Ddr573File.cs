namespace RhythmCodex.Ddr.Models
{
    public class Ddr573File
    {
        public int Id { get; set; }
        public int Offset { get; set; }
        public int Module { get; set; }
        public byte Reserved0 { get; set; }
        public byte Reserved1 { get; set; }
        public byte Reserved2 { get; set; }
        public byte[] Data { get; set; }
    }
}