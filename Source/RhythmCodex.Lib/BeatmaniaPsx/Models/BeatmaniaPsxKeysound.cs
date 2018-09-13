using RhythmCodex.Infrastructure;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.BeatmaniaPsx.Models
{
    [Model]
    public class BeatmaniaPsxKeysound
    {
        public BeatmaniaPsxKeysoundDirectoryEntry DirectoryEntry { get; set; }
        public VagChunk Data { get; set; }
    }
}