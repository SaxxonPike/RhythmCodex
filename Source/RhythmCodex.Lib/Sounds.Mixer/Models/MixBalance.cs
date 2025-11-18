namespace RhythmCodex.Sounds.Mixer.Models;

public record MixBalance(
    MixAmount Sample,
    MixAmount Master
);