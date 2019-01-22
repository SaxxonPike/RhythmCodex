using RhythmCodex.Beatmania.Models;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Beatmania.Converters
{
    public interface IBeatmaniaPcAudioDecoder
    {
        ISound Decode(BeatmaniaPcAudioEntry entry);
    }
}