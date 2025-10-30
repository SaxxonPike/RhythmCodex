using RhythmCodex.Beatmania.Models;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters;

public interface ITwinkleBeatmaniaDecoder
{
    TwinkleArchive? Decode(TwinkleBeatmaniaChunk chunk, TwinkleDecodeOptions options);
    BeatmaniaPcSongSet MigrateToBemaniPc(TwinkleBeatmaniaChunk chunk);
}