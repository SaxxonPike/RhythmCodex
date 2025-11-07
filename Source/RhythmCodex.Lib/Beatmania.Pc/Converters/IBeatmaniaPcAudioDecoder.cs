using RhythmCodex.Beatmania.Pc.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Beatmania.Pc.Converters;

public interface IBeatmaniaPcAudioDecoder
{
    Sound? Decode(BeatmaniaPcAudioEntry entry);
}