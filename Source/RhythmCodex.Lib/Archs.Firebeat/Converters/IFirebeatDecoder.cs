using RhythmCodex.Archs.Firebeat.Models;

namespace RhythmCodex.Archs.Firebeat.Converters;

public interface IFirebeatDecoder
{
    FirebeatArchive? Decode(FirebeatChunk chunk, FirebeatDecodeOptions options);

}