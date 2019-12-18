using RhythmCodex.Infrastructure;

namespace RhythmCodex.Arc.Model
{
    [Model]
    public class ArcFile
    {
        public string Name { get; set; }
        public bool IsCompressed { get; set; }
        public byte[] Data { get; set; }
    }
}