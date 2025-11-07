using RhythmCodex.Archs.Djmain.Model;

namespace RhythmCodex.Archs.Djmain.Converters;

public interface IDjmainDecoder
{
    DjmainArchive Decode(DjmainChunk chunk, DjmainDecodeOptions options);
}