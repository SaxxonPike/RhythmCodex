using System.IO;
using RhythmCodex.Games.Ddr.Ps2.Models;

namespace RhythmCodex.Games.Ddr.Streamers;

public interface IDdrPs2FileDataStepStreamReader
{
    DdrPs2FileDataTableChunk? Read(Stream fileDataBinStream, long length);
}