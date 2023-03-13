using System.IO;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Streamers;

public interface IDdrPs2FileDataStepStreamReader
{
    DdrPs2FileDataTableChunk Read(Stream fileDataBinStream, long length);
}