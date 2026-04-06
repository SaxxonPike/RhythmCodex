using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

/// <summary>
/// Decodes beatmaniaIIDX PS2 streamed BGMs.
/// </summary>
public interface IBeatmaniaPs2BgmDecoder
{
    /// <summary>
    /// Decodes a BGM to the common format.
    /// </summary>
    Sound? Decode(BeatmaniaPs2Bgm bgm);
}