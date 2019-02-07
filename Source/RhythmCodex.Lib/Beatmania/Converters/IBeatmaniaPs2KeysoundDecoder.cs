using RhythmCodex.Beatmania.Models;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Beatmania.Converters
{
    public interface IBeatmaniaPs2KeysoundDecoder
    {
        ISound Decode(BeatmaniaPs2Keysound keysound);
    }
}