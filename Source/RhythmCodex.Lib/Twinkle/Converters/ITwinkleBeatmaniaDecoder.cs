using RhythmCodex.Beatmania.Models;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters;

public interface ITwinkleBeatmaniaDecoder
{
    TwinkleArchive? Decode(TwinkleBeatmaniaChunk chunk);
    BeatmaniaPcSongSet MigrateToBemaniPc(TwinkleBeatmaniaChunk chunk);
}