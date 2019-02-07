using RhythmCodex.Beatmania.Models;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Beatmania.Converters
{
    public interface IBeatmaniaPs2BgmDecoder
    {
        ISound Decode(BeatmaniaPs2Bgm bgm);
    }
}