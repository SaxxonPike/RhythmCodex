using RhythmCodex.Infrastructure;

namespace RhythmCodex.Arc.Model
{
    [Model]
    public class ArcHeader
    {
        public int Id { get; set; }
        public int Unk0 { get; set; }
        public int FileCount { get; set; }
        public int Unk1 { get; set; }
    }
}