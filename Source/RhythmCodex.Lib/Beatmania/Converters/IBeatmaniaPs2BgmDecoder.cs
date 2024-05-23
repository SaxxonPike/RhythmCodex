using RhythmCodex.Beatmania.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Beatmania.Converters;

public interface IBeatmaniaPs2BgmDecoder
{
    Sound? Decode(BeatmaniaPs2Bgm bgm);
}