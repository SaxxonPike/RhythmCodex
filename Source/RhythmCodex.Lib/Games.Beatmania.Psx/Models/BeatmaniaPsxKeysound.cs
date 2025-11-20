using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Games.Beatmania.Psx.Models;

[Model]
public class BeatmaniaPsxKeysound
{
    public BeatmaniaPsxKeysoundDirectoryEntry DirectoryEntry { get; set; }
    public VagChunk? Data { get; set; }
}