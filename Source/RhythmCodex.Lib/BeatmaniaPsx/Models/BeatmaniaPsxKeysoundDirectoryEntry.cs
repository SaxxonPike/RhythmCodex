using RhythmCodex.Infrastructure;

namespace RhythmCodex.BeatmaniaPsx.Models
{
    [Model]
    public class BeatmaniaPsxKeysoundDirectoryEntry
    {
        public int Offset { get; set; }
        public int Unknown0 { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
    }
}