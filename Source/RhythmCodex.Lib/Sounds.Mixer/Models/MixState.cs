using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Mixer.Models;

public record MixState(
    Sound? Sound,
    int SampleOffset,
    MixBalance? Balance,
    Metadata? EventData
);