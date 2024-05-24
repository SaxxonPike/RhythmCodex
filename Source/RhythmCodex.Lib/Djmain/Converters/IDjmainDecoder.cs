using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters;

public interface IDjmainDecoder
{
    DjmainArchive Decode(DjmainChunk chunk, DjmainDecodeOptions options);
}