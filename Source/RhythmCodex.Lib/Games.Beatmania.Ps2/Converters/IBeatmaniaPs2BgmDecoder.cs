using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

public interface IBeatmaniaPs2BgmDecoder
{
    Sound? Decode(BeatmaniaPs2Bgm bgm);
}