namespace RhythmCodex.Sounds.Mixer.Models;

public record MixAmount(
    (double ToLeft, double ToRight) FromLeft,
    (double ToLeft, double ToRight) FromRight
);