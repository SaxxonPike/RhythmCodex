using RhythmCodex.Beatmania.Models;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Beatmania.Converters
{
    public interface IBeatmaniaPs2BgmDecoder
    {
        ISound Decode(BeatmaniaPs2Bgm bgm);
    }
}