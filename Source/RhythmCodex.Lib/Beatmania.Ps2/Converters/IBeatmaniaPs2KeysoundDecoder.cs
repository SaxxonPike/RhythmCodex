using RhythmCodex.Beatmania.Ps2.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Beatmania.Ps2.Converters;

public interface IBeatmaniaPs2KeysoundDecoder
{
    Sound? Decode(BeatmaniaPs2Keysound keysound);
}