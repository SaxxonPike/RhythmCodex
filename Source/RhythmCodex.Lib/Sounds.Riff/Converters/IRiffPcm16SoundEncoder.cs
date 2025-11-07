using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Riff.Models;

namespace RhythmCodex.Sounds.Riff.Converters;

public interface IRiffPcm16SoundEncoder
{
    RiffContainer Encode(Sound? sound);
}