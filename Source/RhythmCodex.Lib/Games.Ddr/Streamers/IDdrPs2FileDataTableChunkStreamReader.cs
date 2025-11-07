using System.IO;
using RhythmCodex.Games.Ddr.Ps2.Models;

namespace RhythmCodex.Games.Ddr.Streamers;

public interface IDdrPs2FileDataTableChunkStreamReader
{
    DdrPs2FileDataTableChunk? GetUnbound(Stream stream);
    DdrPs2FileDataTableChunk GetBound(Stream stream);
}