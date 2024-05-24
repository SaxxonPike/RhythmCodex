using RhythmCodex.Beatmania.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Beatmania.Converters;

public interface IBeatmaniaPcAudioDecoder
{
    Sound? Decode(BeatmaniaPcAudioEntry entry);
}