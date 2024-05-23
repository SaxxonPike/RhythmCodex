using RhythmCodex.Riff.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Riff.Converters;

public interface IRiffMicrosoftAdpcmSoundEncoder
{
    IRiffContainer Encode(ISound sound, int samplesPerBlock);
}