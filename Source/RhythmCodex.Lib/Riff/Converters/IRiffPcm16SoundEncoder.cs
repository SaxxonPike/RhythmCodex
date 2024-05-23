using RhythmCodex.Riff.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Riff.Converters;

public interface IRiffPcm16SoundEncoder
{
    IRiffContainer Encode(Sound? sound);
}