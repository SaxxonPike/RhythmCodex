using RhythmCodex.Infrastructure;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Beatmania.Models;

[Model]
public class BeatmaniaPsxKeysound
{
    public required BeatmaniaPsxKeysoundDirectoryEntry DirectoryEntry { get; set; }
    public VagChunk? Data { get; set; }
}