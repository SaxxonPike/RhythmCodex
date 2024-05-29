using RhythmCodex.Riff.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Riff.Converters;

public interface IRiffMicrosoftAdpcmSoundEncoder
{
    RiffContainer Encode(Sound sound, int samplesPerBlock);
}