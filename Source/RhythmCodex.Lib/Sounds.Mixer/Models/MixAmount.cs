namespace RhythmCodex.Sounds.Mixer.Models;

public record struct MixAmount(
    (double ToLeft, double ToRight) FromLeft,
    (double ToLeft, double ToRight) FromRight
);