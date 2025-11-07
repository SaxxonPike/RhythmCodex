using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Riff.Models;

namespace RhythmCodex.Sounds.Riff.Converters;

public interface IRiffMicrosoftAdpcmSoundEncoder
{
    RiffContainer Encode(Sound sound, int samplesPerBlock);
}