using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

public interface ISsqInfoChunkDecoder
{
    SsqInfoChunk Decode(SsqChunk ssqChunk);
}