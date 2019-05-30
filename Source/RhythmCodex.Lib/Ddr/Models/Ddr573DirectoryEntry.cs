using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ddr.Models
{
    [Model]
    public class Ddr573DirectoryEntry
    {
        public int Id { get; set; }
        public int Offset { get; set; }
        public int Module { get; set; }
        public int CompressionType { get; set; }
        public int Reserved0 { get; set; }
        public int Reserved1 { get; set; }
        public int Reserved2 { get; set; }
        public int Length { get; set; }
    }
}