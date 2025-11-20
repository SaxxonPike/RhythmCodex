using JetBrains.Annotations;
using RhythmCodex.Charts.Ssq.Model;

namespace RhythmCodex.Charts.Ssq.Converters;

[PublicAPI]
public interface ISsqInfoChunkDecoder
{
    SsqInfoChunk Decode(SsqChunk ssqChunk);
}