using System.IO;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Ddr.Ps2.Models;

namespace RhythmCodex.Ddr.Streamers;

public interface IDdrPs2FileDataTableChunkStreamReader
{
    DdrPs2FileDataTableChunk? GetUnbound(Stream stream);
    DdrPs2FileDataTableChunk GetBound(Stream stream);
}