using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Ssq.Converters
{
    public interface ISsqInfoChunkEncoder
    {
        SsqChunk Encode(SsqInfoChunk infoChunk);
    }
}