using RhythmCodex.Infrastructure;

namespace RhythmCodex.Beatmania.Models
{
    [Model]
    public class BeatmaniaPsxFile
    {
        public byte[] Data { get; set; }
    }
}