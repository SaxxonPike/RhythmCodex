using RhythmCodex.Archs.Twinkle.Model;
using RhythmCodex.Games.Beatmania.Pc.Models;

namespace RhythmCodex.Archs.Twinkle.Converters;

public interface ITwinkleBeatmaniaDecoder
{
    TwinkleArchive? Decode(TwinkleBeatmaniaChunk chunk, TwinkleDecodeOptions options);
    BeatmaniaPcSongSet MigrateToBemaniPc(TwinkleBeatmaniaChunk chunk);
}