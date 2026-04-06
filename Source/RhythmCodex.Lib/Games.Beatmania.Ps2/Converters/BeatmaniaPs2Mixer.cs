using RhythmCodex.IoC;
using RhythmCodex.Sounds.Mixer.Converters;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

/// <inheritdoc cref="IBeatmaniaPs2Mixer" />
[Service]
public sealed class BeatmaniaPs2Mixer : DefaultStereoMixer, IBeatmaniaPs2Mixer;