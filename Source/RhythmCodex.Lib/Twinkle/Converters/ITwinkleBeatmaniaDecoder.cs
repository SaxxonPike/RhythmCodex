using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters
{
    public interface ITwinkleBeatmaniaDecoder
    {
        TwinkleArchive Decode(TwinkleBeatmaniaChunk chunk);
    }
}