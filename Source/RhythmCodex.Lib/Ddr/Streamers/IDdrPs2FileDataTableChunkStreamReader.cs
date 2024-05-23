using System.IO;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Streamers;

public interface IDdrPs2FileDataTableChunkStreamReader
{
    DdrPs2FileDataTableChunk GetUnbound(Stream stream);
    DdrPs2FileDataTableChunk GetBound(Stream stream);
}