using RhythmCodex.Beatmania.Models;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Beatmania.Converters
{
    public interface IBeatmaniaPs2KeysoundDecoder
    {
        ISound Decode(BeatmaniaPs2Keysound keysound);
    }
}