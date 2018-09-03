namespace RhythmCodex.Ddr.Models
{
    public class Ddr573DirectoryEntry
    {
        public int Id { get; set; }
        public short Offset { get; set; }
        public short Module { get; set; }
        public byte CompressionType { get; set; }
        public byte Reserved0 { get; set; }
        public byte Reserved1 { get; set; }
        public byte Reserved2 { get; set; }
        public int Length { get; set; }
    }
}