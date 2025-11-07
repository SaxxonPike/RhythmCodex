using RhythmCodex.Games.Beatmania.Pc.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Games.Beatmania.Pc.Converters;

public interface IBeatmaniaPcAudioDecoder
{
    Sound? Decode(BeatmaniaPcAudioEntry entry);
}