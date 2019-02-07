using RhythmCodex.Beatmania.Models;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Beatmania.Converters
{
    public interface IBeatmaniaPcAudioDecoder
    {
        ISound Decode(BeatmaniaPcAudioEntry entry);
    }
}