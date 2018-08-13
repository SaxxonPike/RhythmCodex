using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Converters
{
    public interface IDjmainDecoder
    {
        IDjmainArchive Decode(IDjmainChunk chunk);
    }
}